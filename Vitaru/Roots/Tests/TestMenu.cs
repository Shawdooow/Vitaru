// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus;
using Vitaru.Roots.Menu;
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

        private readonly TrackController controller;

        public TestMenu(Vitaru vitaru)
        {
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

                Disabled = Vitaru.FEATURES < Features.Experimental,
                OnClick = () =>
                {
                    if (Vitaru.FEATURES >= Features.Experimental && TrackManager.CurrentTrack != null)
                        AddRoot(new MultiMenu());
                }
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

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new EditorRoot());
                }
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
            Add(new TrackSelect());

            //Add(new WikiOverlay());
            Add(new SettingsOverlay(vitaru));

            Add(new Version());

            Add(new InstancedText
            {
                Y = -300,
                Text = Vitaru.ALKI > 0 ? Vitaru.ALKI == 2 ? "Rhize" : "Alki" : "Vitaru"
            });

            Renderer.Window.CursorHidden = true;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            controller.PrimeTrackManager();
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.SetTrackDefaults();
            TrackManager.SetPositionalDefaults();
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();
        }

        protected override void DropRoot()
        {
            //base.DropRoot();
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