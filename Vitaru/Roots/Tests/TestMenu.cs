// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Vitaru.Roots.Multi;
using Vitaru.Settings;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : MenuRoot
    {
        public override string Name => nameof(TestMenu);

        protected override bool Parallax => true;

        private readonly Box cursor;

        private readonly TrackController controller;

        public TestMenu(Vitaru vitaru)
        {
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
            Add(Back = new Exit(vitaru));

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
        }

        private class Exit : Button
        {
            public Exit(Vitaru vitaru)
            {
                ParentOrigin = Mounts.BottomLeft;
                Origin = Mounts.BottomLeft;
                Position = new Vector2(10, -10);
                Size = new Vector2(80, 40);

                Background = Game.TextureStore.GetTexture("square.png");
                BackgroundSprite.Color = Color.Red;

                Text = "Exit";

                OnClick = vitaru.Exit;
            }

            protected override void Flash()
            {
                //Don't do it because it crashes
                //base.Flash();
            }
        }
    }
}