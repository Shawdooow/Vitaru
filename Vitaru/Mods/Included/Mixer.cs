// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Utilities;
using Vitaru.Roots;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Mods.Included
{
    public class Mixer : Mod
    {
        public override Button GetMenuButton() =>
            new Button
            {
                Y = -180,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta
                },

                Text = "Mixer"
            };

        public override Root GetRoot() => new MixerRoot();

        private class MixerRoot : MenuRoot
        {
            public override string Name => nameof(MixerRoot);

            protected override bool UseLevelBackground => true;

            protected override bool Parallax => true;

            private const float min = 0.05f;
            private const float max = 2f;

            private float rate = 1f;
            private float gain = 1f;

            private TrackController controller;

            private InstancedText song;

            private InstancedText volume;
            private Slider control;

            private InstancedText pitch;
            private Slider slider;

            private Button play;
            private InstancedText timeIn;
            private Slider seek;
            private InstancedText timeLeft;

            private bool accel;

            public override void LoadingComplete()
            {
                AddArray(new ILayer[]
                {
                    song = new InstancedText
                    {
                        Y = 16,
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,
                        FontScale = 0.6f,
                        Text = TrackManager.CurrentTrack.Level.Title
                    },

                    new Button
                    {
                        X = 360,
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "++",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.25f)
                    },
                    new Button
                    {
                        Position = new Vector2(240, 0),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "1.5x",
                        OnClick = () => setRate(1.5f)
                    },
                    new Button
                    {
                        Position = new Vector2(120, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "+",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.05f)
                    },
                    new Button
                    {
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "1x",
                        OnClick = () => setRate(1f)
                    },
                    new Button
                    {
                        Position = new Vector2(-120, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "-",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.05f)
                    },
                    new Button
                    {
                        Position = new Vector2(-240, 0),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },

                        Text = "0.75x",
                        OnClick = () => setRate(0.75f)
                    },
                    new Button
                    {
                        Position = new Vector2(-360, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.5f
                        },
                        Text = "--",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.25f)
                    },

                    new Button
                    {
                        Size = new Vector2(80, 40),
                        Y = 80,

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        InstancedText =
                        {
                            FontScale = 0.25f
                        },

                        Text = "Accel",
                        OnClick = () => accel = !accel
                    },

                    pitch = new InstancedText
                    {
                        Position = new Vector2(0, -160),
                        Text = TrackManager.CurrentTrack.Pitch.ToString()
                    },
                    slider = new Slider
                    {
                        Width = 1000,
                        Position = new Vector2(0, -100),
                        OnProgressInput = p => setRate(PrionMath.Scale(p, 0, 1, min, max))
                    },

                    volume = new InstancedText
                    {
                        Position = new Vector2(0, -280),
                        FontScale = 0.5f,
                        Text = (TrackManager.CurrentTrack.Gain * 100).ToString()
                    },
                    control = new Slider
                    {
                        Width = 200,
                        Position = new Vector2(0, -240),
                        OnProgressInput = setVolume
                    },

                    seek = new Slider
                    {
                        Y = -60,
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Width = 800,
                        OnProgressInput = p =>
                            TrackManager.CurrentTrack.Seek(
                                PrionMath.Scale(p, 0, 1, 0, TrackManager.CurrentTrack.Length))
                    },

                    play = new Button
                    {
                        Position = new Vector2(0, -120),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(64),
                        Background = Vitaru.TextureStore.GetTexture("pause.png"),

                        OnClick = toggle
                    },
                    new Button
                    {
                        Position = new Vector2(72, -120),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(64),
                        Background = Vitaru.TextureStore.GetTexture("skip.png"),

                        OnClick = next
                    },
                    new Button
                    {
                        Position = new Vector2(-72, -120),
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,
                        Size = new Vector2(-64, 64),
                        Background = Vitaru.TextureStore.GetTexture("skip.png")
                    },

                    controller = new TrackController
                    {
                        Alpha = 0,
                        PassDownInput = false
                    }
                });

                slider.AddArray(new IDrawable2D[]
                {
                    new InstancedText
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f,
                        Text = "0.05x"
                    },
                    new InstancedText
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterLeft,
                        X = 12,
                        FontScale = 0.25f,
                        Text = "2x"
                    }
                });

                seek.AddArray(new IDrawable2D[]
                {
                    timeIn = new InstancedText
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterRight,
                        X = -12,
                        FontScale = 0.25f
                    },
                    timeLeft = new InstancedText
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterLeft,
                        X = 12,
                        FontScale = 0.25f
                    }
                });

                setRate(TrackManager.CurrentTrack.Pitch);
                setVolume(TrackManager.CurrentTrack.Gain);

                TrackManager.OnTrackChange += change;

                Add(new TrackSelect());

                base.LoadingComplete();
            }

            public override void Update()
            {
                base.Update();

                controller.Update();
                controller.TryNextLevel();

                float current = (float) TrackManager.CurrentTrack.Clock.Current;
                float length = (float) TrackManager.CurrentTrack.Length * 1000;

                if (!seek.Dragging)
                    seek.Progress = PrionMath.Scale(current, 0, length);

                if (accel)
                    setRate(PrionMath.Scale(current, 0, length, 0.75f, 1.5f));

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
                pitch.Text = $"{MathF.Round(r, 2)}x";
                slider.Progress = PrionMath.Scale(rate, min, max);
            }

            private void setVolume(float v)
            {
                TrackManager.CurrentTrack.Gain = gain = Math.Clamp(v, 0, 1);
                volume.Text = $"{MathF.Round(gain * 100, 0)}%";
                control.Progress = PrionMath.Scale(gain, 0, 1);
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

            private void change(Track t) => song.Text = t.Level.Title;

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
                    case Keys.NextTrack:
                        //play.Background = Vitaru.TextureStore.GetTexture("play.png");
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