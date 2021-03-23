// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
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
using Vitaru.Server.Levels;
using Vitaru.Themes;

namespace Vitaru.Tracks
{
    public class TrackController : InputLayer<IDrawable2D>, IHasInputKeys
    {
        private readonly MaskingLayer<Sprite> mask;
        private readonly Sprite background;

        private readonly Button play;
        private readonly InstancedText song;
        private readonly InstancedText speed;
        private readonly InstancedText volume;

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
                mask = new MaskingLayer<Sprite>
                {
                    Children = new[]
                    {
                        background = new Sprite
                        {
                            AutoScaleDirection = Direction.Both,
                            Size = Size,
                            Texture = ThemeManager.GetBackground()
                        }
                    },

                    Masks = new Sprite[]
                    {
                        //new Box
                        //{
                        //    Origin = Mounts.BottomCenter,
                        //    ParentOrigin = Mounts.TopCenter,
                        //    Alpha = 0f,
                        //    Size = Size,
                        //    Y = -Height / 2f + 0.5f,
                        //},
                        new Box
                        {
                            //Origin = Mounts.TopCenter,
                            //ParentOrigin = Mounts.BottomCenter,
                            Alpha = 1f,
                            Size = Size
                            //Y = Height / 2f - 0.5f,
                        }
                    }
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
                    Background = Vitaru.TextureStore.GetTexture("skip.png"),

                    OnClick = PreviousLevel
                },
                song = new InstancedText
                {
                    Position = new Vector2(4),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    FontScale = 0.2f,
                    Text = "Loading..."
                },
                speed = new InstancedText
                {
                    Position = new Vector2(-16),
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,
                    FontScale = 0.2f,
                    Text = "1x"
                },
                volume = new InstancedText
                {
                    Position = new Vector2(16, -16),
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,
                    FontScale = 0.2f,
                    Text = "100%"
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

            speed.Text = $"{Math.Round(TrackManager.CurrentTrack.Pitch, 2)}x";
            volume.Text = $"{Math.Round(TrackManager.CurrentTrack.Gain * 100, 2)}%";
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty)
            {
                background.Texture =
                    bg == "default" ? ThemeManager.GetBackground() : Vitaru.LevelTextureStore.GetTexture(bg);

                //float height = (background.DrawSize.Y - Height) / 2;

                //mask.Masks[0].Height = height;
                //mask.Masks[1].Height = height;

                bg = string.Empty;
            }
        }

        public void PrimeTrackManager()
        {
            Game.ScheduleLoad(() =>
            {
                Benchmark track = new("Prime TrackManager", true);

                LevelPack p = LevelStore.GetRandomLevel(null);

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
                Benchmark b = new("Switch to Next Level", true);

                TrackManager.PreviousLevels.Push(LevelStore.CurrentLevel.LevelTrack);

                LevelTrack n = LevelStore.SetRandomLevel(LevelStore.CurrentPack);
                song.Text = $"Loading: {n.Title}";

                TrackManager.SetTrack(n);
                
                b.Finish();
            });
        }

        public void PreviousLevel()
        {
            if (TrackManager.Switching || !TrackManager.PreviousLevels.Any()) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new("Switch to Previous Level", true);

                LevelTrack previous = TrackManager.PreviousLevels.Pop();
                LevelStore.SetLevel(LevelStore.GetLevelPack(previous));
                song.Text = $"Loading: {previous.Title}";

                TrackManager.SetTrack(previous);
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
                case Keys.PreviousTrack:
                    PreviousLevel();
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