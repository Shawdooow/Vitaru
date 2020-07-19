// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
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
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Roots.Multi;
using Vitaru.Settings;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        public override string Name => nameof(TestMenu);

        private const float parallax = 10;

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
                        Size = new Vector2(Renderer.Width + parallax, Renderer.Height + parallax),
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

            Button multi;
            Button edit;

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
            Add(multi = new Button
            {
                Y = -60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.SecondaryColor
                },

                Text = "Multi"
            });
            Add(edit = new Button
            {
                Y = 60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.TrinaryColor
                },

                Text = "Edit"
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

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new ModsTest());
                }
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

            Add(controller = new TrackController());

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

            if (!Vitaru.EXPERIMENTAL)
            {
                multi.Add(new Box
                {
                    Size = multi.Size,
                    Scale = multi.Scale,
                    Color = Color.Black,
                    Alpha = 0.5f
                });
                edit.Add(new Box
                {
                    Size = multi.Size,
                    Scale = multi.Scale,
                    Color = Color.Black,
                    Alpha = 0.5f
                });
            }
            else
            {
                multi.OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new MultiMenu());
                };
                edit.OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new EditorTest());
                };
            }
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            controller.PrimeTrackManager();
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.CurrentTrack.Pitch = 1;
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();

            cursor.Position = InputManager.Mouse.Position;

            Vector2 min = new Vector2(-parallax / 2);
            Vector2 max = new Vector2(parallax / 2);

            Vector2 p = PrionMath.Scale(InputManager.Mouse.Position,
                new Vector2(Renderer.Width / -2f, Renderer.Height / -2f),
                new Vector2(Renderer.Width / 2f, Renderer.Height / 2f), min, max);

            Background.Position = PrionMath.Clamp(p, min, max);
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            Background.Size = new Vector2(size.X + parallax, size.Y + parallax);
            Dim.Size = size;
        }
    }
}