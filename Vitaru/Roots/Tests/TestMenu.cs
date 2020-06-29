// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Overlays;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UserInterface;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Levels;
using Vitaru.Roots.Multi;
using Vitaru.Server.Track;
using Vitaru.Settings;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        public override string Name => nameof(TestMenu);

        private readonly Box cursor;

        protected readonly Sprite Background;
        protected readonly Box Dim;

        private readonly Button next;
        private readonly SpriteText song;

        private readonly SeekableClock seek;

        public TestMenu(Vitaru vitaru)
        {
            seek = new SeekableClock();

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    Background = new Sprite(Vitaru.GetBackground())
                    {
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    },
                    Dim = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    }
                }
            });

            Add(new Button
            {
                Y = -180,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.ForestGreen
                },

                Text = "Play",

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null) 
                        AddRoot(new PlayTest(seek));
                }
            });
            Add(new Button
            {
                Y = -60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta
                },

                Text = "Multi",

                OnClick = () => AddRoot(new MultiMenu())
            });
            Add(new Button
            {
                Y = 60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkTurquoise
                },

                Text = "Edit",

                OnClick = () => AddRoot(new EditorTest())
            });
            Add(new Button
            {
                Y = 180,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkGoldenrod
                },

                Text = "Mods",

                OnClick = () => AddRoot(new ModsTest())
            });
            Add(new Button
            {
                ParentOrigin = Mounts.BottomLeft,
                Origin = Mounts.BottomLeft,
                Position = new Vector2(10, -10),
                Size = new Vector2(80, 40),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.Red
                },

                Text = "Exit",

                OnClick = vitaru.Exit
            });

            Add(next = new Button
            {
                Position = new Vector2(-10, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Size = new Vector2(160, 90),

                Dim =
                {
                    Alpha = 0.8f
                },
                SpriteText =
                {
                    TextScale = 0.25f
                },

                Text = "Next",

                OnClick = nextLevel
            });

            //Add(new WikiOverlay());
            Add(new SettingsOverlay());

            Add(song = new SpriteText
            {
                Position = new Vector2(-10, 10),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f,
                Text = "Loading..."
            });

            Add(new SpriteText
            {
                Position = new Vector2(10),
                ParentOrigin = Mounts.TopLeft,
                Origin = Mounts.TopLeft,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru"
            });

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    cursor = new Box
                    {
                        Color = Color.Red,
                        Alpha = 0.25f,
                        Size = new Vector2(10)
                    }
                }
            });

            Add(new FPSOverlay());
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();

            TrackManager.OnTrackChange += t =>
            {
                song.Text = $"Now Playing: {t.Level.Name}";

                if (t.Level.Image != string.Empty)
                    bg = $"{t.Level.Name}\\{t.Level.Image}";
            };

            qued = true;
            Game.ScheduleLoad(() =>
            {
                Benchmark track = new Benchmark("Prime TrackManager", true);

                LevelTrack t = LevelStore.LoadedLevels[PrionMath.RandomNumber(0, LevelStore.LoadedLevels.Count)].Levels[0]
                    .LevelTrack;
                TrackManager.SetTrack(t, seek);

                track.Finish();

                qued = false;
            });
        }

        protected override void OnResume()
        {
            base.OnResume();
            seek.Rate = 1;
            TrackManager.CurrentTrack.Pitch = 1;
        }

        private bool qued;

        private string bg = string.Empty;

        public override void Update()
        {
            seek.NewFrame();
            if (TrackManager.CurrentTrack != null)
            {
                if (TrackManager.CurrentTrack.CheckFinish())
                    nextLevel();
            }

            base.Update();

            cursor.Position = InputManager.Mouse.Position;
        }

        private void nextLevel()
        {
            if (qued) return;

            qued = true;
            Game.ScheduleLoad(() =>
            {
                Benchmark b = new Benchmark("Switch Level", true);
                TrackManager.NextTrack();
                b.Finish();
                qued = false;
            });
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty)
            {
                next.Background = Vitaru.LevelTextureStore.GetTexture(bg);
                bg = string.Empty;
            }
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            Background.Size = size;
            Dim.Size = size;
        }
    }
}