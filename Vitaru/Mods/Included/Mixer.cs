// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Audio.Contexts;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Roots;
using Vitaru.Roots.Menu;
using Vitaru.Tracks;

namespace Vitaru.Mods.Included
{
    public class Mixer : Mod
    {
        public override string Name => nameof(Mixer);

        public override Button GetMenuButton() =>
            new()
            {
                Y = -180,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta,
                },

                Text = "Mixer",
            };

        public override Root GetRoot() => new MixerRoot();

        public class MixerRoot : MenuRoot
        {
            public override string Name => nameof(MixerRoot);

            protected override bool UseLevelBackground => true;

            protected override bool Parallax => true;

            private const float min = 0.05f;
            private const float max = 2f;

            private float rate = 1f;
            private float gain = 1f;

            private VitaruTrackController controller;

            private Text2D song;

            protected Text2D Volume;
            protected Slider Control;

            protected Text2D Pitch;
            protected Slider Slider;

            private Button play;
            private Text2D timeIn;
            private Slider seek;
            private Text2D timeLeft;

            private Slider accelMin;
            private Text2D accelMinTime;
            private Slider accelMax;
            private Text2D accelMaxTime;

            private bool accel;

            public override void LoadingComplete()
            {
                AddArray(new ILayer[]
                {
                    song = new Text2D
                    {
                        Y = 16,
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,
                        FontScale = 0.6f,
                        Text = TrackManager.CurrentTrack.Metadata.Title,
                    },

                    new Button
                    {
                        Position = new Vector2(240, 140),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.5f,
                        },

                        Text = "1.5x",
                        OnClick = () => setRate(1.5f),
                    },
                    new Button
                    {
                        Position = new Vector2(120, 140),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.5f,
                        },

                        Text = "+",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.05f),
                        OnRightClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.25f),
                    },
                    new Button
                    {
                        Position = new Vector2(0, 140),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.5f,
                        },

                        Text = "1x",
                        OnClick = () => setRate(1f),
                    },
                    new Button
                    {
                        Position = new Vector2(-120, 140),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.5f,
                        },

                        Text = "-",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.05f),
                        OnRightClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.25f),
                    },
                    new Button
                    {
                        Position = new Vector2(-240, 140),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.5f,
                        },

                        Text = "0.75x",
                        OnClick = () => setRate(0.75f),
                    },

                    new Button
                    {
                        Position = new Vector2(120, 220),
                        Size = new Vector2(80, 40),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.25f,
                        },

                        Text = "Accel",
                        OnClick = () => accel = !accel,
                    },
                    new Button
                    {
                        Position = new Vector2(-120, 220),
                        Size = new Vector2(80, 40),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f,
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor,
                        },
                        Text2D =
                        {
                            FontScale = 0.25f,
                        },

                        Text = "Deccel",
                        Disabled = true,
                        //OnClick = () => accel = !accel,
                    },

                    Pitch = new Text2D
                    {
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Position = new Vector2(300, -180),
                        FontScale = 0.25f,
                        Text = TrackManager.CurrentTrack.Pitch.ToString(),
                    },
                    Slider = new Slider
                    {
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 200,
                        Position = new Vector2(300, -150),
                        OnProgressInput = p => setRate(PrionMath.Remap(p, 0, 1, min, max)),
                    },

                    Volume = new Text2D
                    {
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Position = new Vector2(-300, -180),
                        FontScale = 0.25f,
                        Text = (TrackManager.CurrentTrack.Gain * 100).ToString(),
                    },
                    Control = new Slider
                    {
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 200,
                        Position = new Vector2(-300, -150),
                        OnProgressInput = setVolume,
                    },

                    seek = new Slider
                    {
                        Y = -75,
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 800,
                        OnProgressInput = p =>
                            TrackManager.CurrentTrack.Seek(
                                PrionMath.Remap(p, 0, 1, 0, TrackManager.CurrentTrack.Sample.Length)),
                    },
                    accelMin = new Slider
                    {
                        Y = -110,
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 800,
                        OnProgressInput = aMin,
                    },
                    accelMax = new Slider
                    {
                        Y = -40,
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 800,
                        Progress = 1,
                        OnProgressInput = aMax,
                    },

                    play = new Button
                    {
                        Position = new Vector2(0, -140),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(64),
                        Background =
                            Vitaru.TextureStore.GetTexture(TrackManager.CurrentTrack.Playing
                                ? "pause.png"
                                : "play.png"),

                        OnClick = toggle,
                    },
                    new Button
                    {
                        Position = new Vector2(72, -140),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(64),
                        Background = Vitaru.TextureStore.GetTexture("skip.png"),

                        OnClick = next,
                    },
                    new Button
                    {
                        Position = new Vector2(-72, -140),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(-64, 64),
                        Background = Vitaru.TextureStore.GetTexture("skip.png"),

                        OnClick = previous,
                    },

                    controller = new VitaruTrackController
                    {
                        Alpha = 0,
                        PassDownInput = false,
                    },
                });

                Slider.AddArray(new IDrawable2D[]
                {
                    new Text2D
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f,
                        Text = "0.05x",
                    },
                    new Text2D
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterLeft,
                        X = 12,
                        FontScale = 0.25f,
                        Text = "2x",
                    },
                });

                seek.AddArray(new IDrawable2D[]
                {
                    timeIn = new Text2D
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f,
                    },
                    timeLeft = new Text2D
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterLeft,
                        X = 12,
                        FontScale = 0.25f,
                    },
                });
                accelMin.AddArray(new IDrawable2D[]
                {
                    accelMinTime = new Text2D
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f,
                    },
                });
                accelMax.AddArray(new IDrawable2D[]
                {
                    accelMaxTime = new Text2D
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f,
                    },
                });

                setRate(TrackManager.CurrentTrack.Pitch);
                setVolume(TrackManager.CurrentTrack.Gain);

                TrackManager.OnTrackChange += change;

                Add(new TrackSelect());
                Add(new LevelSelect());

                if (Vitaru.AssetStorage.Exists("Sounds\\SoundboardSamples"))
                {
                    Sound snare = new Sound(new SeekableClock(), Vitaru.SampleStore.GetSample("SoundboardSamples\\Snare.wav"));
                    Sound snarebell = new Sound(new SeekableClock(), Vitaru.SampleStore.GetSample("SoundboardSamples\\SnareBell.wav"));
                    Sound snarebass = new Sound(new SeekableClock(), Vitaru.SampleStore.GetSample("SoundboardSamples\\SnareBass.wav"));
                    Sound snareclap = new Sound(new SeekableClock(), Vitaru.SampleStore.GetSample("SoundboardSamples\\SnareClap.wav"));

                    AddArray(new ILayer[]
                    {
                        new BindableButton
                        {
                            Bind = Keys.One,

                            Position = new Vector2(-180, -250),
                            Size = new Vector2(100, 100),

                            Background = Game.TextureStore.GetTexture("square.png"),
                            Dim =
                            {
                                Alpha = 0.5f,
                            },
                            BackgroundSprite =
                            {
                                Color = ThemeManager.PrimaryColor,
                            },
                            Text2D =
                            {
                                FontScale = 0.5f,
                            },

                            Text = "S",
                            OnClick = () =>
                            {
                                snare.Pause();
                                snare.Seek(0);
                                snare.Play();
                            },
                        },
                        new BindableButton
                        {
                            Bind = Keys.Two,

                            Position = new Vector2(-60, -250),
                            Size = new Vector2(100, 100),

                            Background = Game.TextureStore.GetTexture("square.png"),
                            Dim =
                            {
                                Alpha = 0.5f,
                            },
                            BackgroundSprite =
                            {
                                Color = ThemeManager.PrimaryColor,
                            },
                            Text2D =
                            {
                                FontScale = 0.5f,
                            },

                            Text = "Snare",
                            OnClick = () =>
                            {
                                snarebell.Pause();
                                snarebell.Seek(0);
                                snarebell.Play();
                            },
                        },
                        new BindableButton
                        {
                            Bind = Keys.Three,

                            Position = new Vector2(60, -250),
                            Size = new Vector2(100, 100),

                            Background = Game.TextureStore.GetTexture("square.png"),
                            Dim =
                            {
                                Alpha = 0.5f,
                            },
                            BackgroundSprite =
                            {
                                Color = ThemeManager.PrimaryColor,
                            },
                            Text2D =
                            {
                                FontScale = 0.5f,
                            },

                            Text = "Snare",
                            OnClick = () =>
                            {
                                snarebass.Pause();
                                snarebass.Seek(0);
                                snarebass.Play();
                            },
                        },
                        new BindableButton
                        {
                            Bind = Keys.Four,

                            Position = new Vector2(180, -250),
                            Size = new Vector2(100, 100),

                            Background = Game.TextureStore.GetTexture("square.png"),
                            Dim =
                            {
                                Alpha = 0.5f,
                            },
                            BackgroundSprite =
                            {
                                Color = ThemeManager.PrimaryColor,
                            },
                            Text2D =
                            {
                                FontScale = 0.5f,
                            },

                            Text = "Snare",
                            OnClick = () =>
                            {
                                snareclap.Pause();
                                snareclap.Seek(0);
                                snareclap.Play();
                            },
                        },
                    });
                }

                base.LoadingComplete();

                aMax(1);
                aMin(0);
            }

            public override void Update()
            {
                base.Update();

                controller.Update();
                controller.TryNextLevel();

                float current = (float)TrackManager.CurrentTrack.Clock.Current;
                float length = (float)TrackManager.CurrentTrack.Sample.Length * 1000;

                if (!seek.Dragging)
                    seek.Progress = PrionMath.Remap(current, 0, length);

                if (accel)
                {
                    float mn = PrionMath.Remap(accelMin.Progress, 0, 1, 0, length);
                    float mx = PrionMath.Remap(accelMax.Progress, 0, 1, 0, length);

                    setRate(Math.Clamp(PrionMath.Remap(current, mn, mx, 0.75f, 1.5f), 0.75f, 1.5f));
                }

                TimeSpan t = TimeSpan.FromMilliseconds(current);
                TimeSpan l = TimeSpan.FromMilliseconds(length - current);

                string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
                string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

                timeIn.Text = time;
                timeLeft.Text = left;
            }

            protected override void TrackChange(Track t)
            {
                base.TrackChange(t);
                t.Pitch = rate;
                t.Gain = gain;
            }

            private void setRate(float r)
            {
                TrackManager.CurrentTrack.Pitch = rate = Math.Clamp(r, min, max);
                Pitch.Text = $"{MathF.Round(rate, 2)}x";
                Slider.Progress = PrionMath.Remap(rate, min, max);
            }

            private void setVolume(float v)
            {
                TrackManager.CurrentTrack.Gain = gain = Math.Clamp(v, 0, 1);
                Volume.Text = $"{MathF.Round(gain * 100, 0)}%";
                Control.Progress = PrionMath.Remap(gain, 0, 1);
            }

            private void toggle()
            {
                if (TrackManager.CurrentTrack.Playing)
                {
                    TrackManager.CurrentTrack.Pause();
                    play.Background = Vitaru.TextureStore.GetTexture("play.png");
                }
                else
                {
                    TrackManager.CurrentTrack.Play();
                    play.Background = Vitaru.TextureStore.GetTexture("pause.png");
                }
            }

            private void next() => controller.NextLevel();

            private void previous() => controller.PreviousLevel();

            private void change(Track t) => song.Text = t.Metadata.Title;

            private void aMax(float p)
            {
                accelMax.Progress = Math.Max(p, accelMin.Progress);

                float current = PrionMath.Remap(p, 0, 1, 0,
                    (float)TrackManager.CurrentTrack.Sample.Length * 1000);

                TimeSpan t = TimeSpan.FromMilliseconds(current);

                string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";

                accelMaxTime.Text = time;
            }

            private void aMin(float p)
            {
                accelMin.Progress = Math.Min(p, accelMax.Progress);

                float current = PrionMath.Remap(p, 0, 1, 0,
                    (float)TrackManager.CurrentTrack.Sample.Length * 1000);

                TimeSpan t = TimeSpan.FromMilliseconds(current);

                string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";

                accelMinTime.Text = time;
            }

            protected override void OnKeyDown(KeyboardKeyEvent e)
            {
                base.OnKeyDown(e);

                switch (e.Key)
                {
                    case Keys.PlayPause:
                        //Yes this is inverted, it is delayed because TrackController gets the event after us...
                        play.Background =
                            Vitaru.TextureStore.GetTexture(TrackManager.CurrentTrack.Playing
                                ? "play.png"
                                : "pause.png");
                        break;
                    case Keys.Stop:
                        play.Background = Vitaru.TextureStore.GetTexture("play.png");
                        break;
                }
            }

            protected override void Dispose(bool finalize)
            {
                base.Dispose(finalize);
                TrackManager.OnTrackChange -= change;
            }
        }
    }
}