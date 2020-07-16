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
using Vitaru.Roots.Multi;
using Vitaru.Settings;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        public override string Name => nameof(TestMenu);

        private readonly Box cursor;

        protected readonly Sprite Background;
        protected readonly Box Dim;

        private readonly TrackController controller;

        public TestMenu(Vitaru vitaru)
        {
            Add(new SpriteLayer
            {
                Children = new[]
                {
                    Background = new Sprite(ThemeManager.GetBackground())
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
                    Color = ThemeManager.PrimaryColor
                },

                Text = "Play",

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new PlayTest());
                }
            });
            Add(new Button
            {
                Y = -60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.SecondaryColor
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
                    Color = ThemeManager.TrinaryColor
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
                    Color = ThemeManager.QuadnaryColor
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

            controller = new TrackController();
            Add(controller);

            //Add(new WikiOverlay());
            Add(new SettingsOverlay());

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
            controller.PrimeTrackManager();
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.SeekableClock.Rate = 1;
            TrackManager.CurrentTrack.Pitch = 1;
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();

            cursor.Position = InputManager.Mouse.Position;
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            Background.Size = size;
            Dim.Size = size;
        }
    }
}