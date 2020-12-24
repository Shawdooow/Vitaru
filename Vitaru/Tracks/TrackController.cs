﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Input.Receivers;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Levels;
using Vitaru.Server.Track;
using Vitaru.Themes;

namespace Vitaru.Tracks
{
    public class TrackController : InputLayer<IDrawable2D>, IHasInputKeys
    {
        private readonly Sprite background;
        private readonly Button play;
        private readonly InstancedText song;

        private readonly InstancedText timeIn;
        private readonly Slider seek;
        private readonly InstancedText timeLeft;

        private string bg = string.Empty;

        public TrackController()
        {
            Position = new Vector2(-20, 20);
            Size = new Vector2(300, 150);
            ParentOrigin = Mounts.TopRight;
            Origin = Mounts.TopRight;

            Vitaru.TextureStore.GetTexture("play.png");

            AddArray(new IDrawable2D[]
            {
                background = new Sprite
                {
                    //TODO: requires masking... AutoScaleDirection = Direction.Both,
                    Size = Size,
                    Texture = ThemeManager.GetBackground()
                },
                new Box
                {
                    Alpha = 0.6f,
                    Size = Size,
                    Color = Color.Black
                },
                play = new Button
                {
                    Position = new Vector2(0, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(32),
                    Background = Vitaru.TextureStore.GetTexture("pause.png"),

                    OnClick = TogglePlay
                },
                new Button
                {
                    Position = new Vector2(32, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(32),
                    Background = Vitaru.TextureStore.GetTexture("skip.png"),

                    OnClick = NextLevel
                },
                new Button
                {
                    Position = new Vector2(-32, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(-32, 32),
                    Background = Vitaru.TextureStore.GetTexture("skip.png")
                },
                song = new InstancedText
                {
                    Position = new Vector2(4),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    FontScale = 0.2f,
                    Text = "Loading..."
                },
                seek = new Slider
                {
                    Width = Size.X,
                    OnProgressInput = p =>
                        TrackManager.CurrentTrack.Seek(PrionMath.Remap(p, 0, 1, 0,
                            TrackManager.CurrentTrack.Sample.Length))
                }
            });

            seek.AddArray(new IDrawable2D[]
            {
                timeIn = new InstancedText
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.BottomLeft,
                    Position = new Vector2(8, -8),
                    FontScale = 0.2f
                },
                timeLeft = new InstancedText
                {
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.BottomRight,
                    Position = new Vector2(-8),
                    FontScale = 0.2f
                }
            });
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            TrackManager.OnTrackChange += change;

            if (TrackManager.CurrentTrack != null)
                change(TrackManager.CurrentTrack);
        }

        //Having a drawable and updatable is bad practice, however here it just makes sense because its a UI element
        public void Update()
        {
            if (TrackManager.CurrentTrack == null) return;

            TrackManager.CurrentTrack.Clock.Update();

            float current = (float) TrackManager.CurrentTrack.Clock.Current;
            float length = (float) TrackManager.CurrentTrack.Sample.Length * 1000;

            if (!seek.Dragging)
                seek.Progress = PrionMath.Remap(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            timeLeft.Text = left;
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty)
            {
                background.Texture =
                    bg == "default" ? ThemeManager.GetBackground() : Vitaru.LevelTextureStore.GetTexture(bg);
                bg = string.Empty;
            }
        }

        public void PrimeTrackManager()
        {
            Game.ScheduleLoad(() =>
            {
                Benchmark track = new Benchmark("Prime TrackManager", true);

                LevelPack p = LevelStore.LoadedLevels[PrionMath.RandomNumber(0, LevelStore.LoadedLevels.Count)];

                if (Vitaru.ALKI == 1)
                {
                    for (int i = 0; i < LevelStore.LoadedLevels.Count; i++)
                        if (LevelStore.LoadedLevels[i].Title == "Alki Bells")
                            p = LevelStore.LoadedLevels[i];
                }
                else if (Vitaru.ALKI == 2)
                {
                    for (int i = 0; i < LevelStore.LoadedLevels.Count; i++)
                        if (LevelStore.LoadedLevels[i].Title == "Alki (All Rhize Remix)")
                            p = LevelStore.LoadedLevels[i];
                }

                LevelStore.SetLevel(p);

                LevelTrack t = LevelStore.CurrentLevel.LevelTrack;
                song.Text = $"Loading: {t.Title}";

                TrackManager.SetTrack(t, new SeekableClock());
                
                TrackManager.SetTrackDefaults();
                TrackManager.SetPositionalDefaults();

                track.Finish();
            });
        }


        public void TryRepeat()
        {
            if (TrackManager.CurrentTrack != null)
                TrackManager.TryRepeatTrack();
        }

        public void TryNextLevel()
        {
            if (TrackManager.CurrentTrack != null)
            {
                if (TrackManager.CurrentTrack.CheckFinish())
                    NextLevel();
            }
        }

        public void TogglePlay()
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

        public void NextLevel()
        {
            if (TrackManager.Switching) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new Benchmark("Switch Level", true);

                LevelTrack n = LevelStore.SetRandomLevel(TrackManager.CurrentTrack.Level);
                song.Text = $"Loading: {n.Title}";

                TrackManager.SetTrack(n);
                b.Finish();
            });
        }

        private void change(Track t)
        {
            song.Text = $"{t.Level.Title}";

            if (t.Level.Image != string.Empty)
                bg = $"{t.Level.Title}\\{t.Level.Image}";
            else
                bg = "default";
        }

        public override void OnResume()
        {
            base.OnResume();
            play.Background =
                Vitaru.TextureStore.GetTexture(TrackManager.CurrentTrack.Playing ? "pause.png" : "play.png");
        }

        public bool OnKeyDown(KeyboardKeyEvent e)
        {
            switch (e.Key)
            {
                default:
                    return false;
                case Keys.PlayPause:
                    TogglePlay();
                    return true;
                case Keys.NextTrack:
                    NextLevel();
                    return true;
                case Keys.Stop:
                    TrackManager.CurrentTrack.Pause();
                    play.Background = Vitaru.TextureStore.GetTexture("play.png");
                    return true;
            }
        }

        public bool OnKeyUp(KeyboardKeyEvent e)
        {
            return true;
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            TrackManager.OnTrackChange -= change;
        }
    }
}