// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus;
using Vitaru.Levels;
using Vitaru.Roots.Menu;
using Vitaru.Roots.Multi;
using Vitaru.Roots.Tests;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class MainMenu : MenuRoot
    {
        public override string Name => nameof(MainMenu);

        protected override bool Parallax => true;

        private readonly VitaruTrackController controller;

        private const int x = 100;
        private const int y = 50;
        private const int width = 180;
        private const int height = 80;

        public MainMenu(Vitaru vitaru)
        {
            Add(new Button
            {
                Position = new Vector2(-x, -y - height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.PrimaryColor
                },

                Text = "Levels",

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new LevelRoot());
                }
            });
            Add(new Button
            {
                Position = new Vector2(x, -y - height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.SecondaryColor
                },

                Text = "Multi",

                Disabled = Vitaru.FEATURES < Features.Radioactive,

                OnClick = () =>
                {
                    if (Vitaru.FEATURES >= Features.Radioactive && TrackManager.CurrentTrack != null)
                        AddRoot(new MultiMenu());
                }
            });
            Add(new Button
            {
                Position = new Vector2(-x, 0),
                Size = new Vector2(width, height),

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
                Position = new Vector2(x, 0),
                Size = new Vector2(width, height),

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
                Position = new Vector2(-x, y + height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.SecondaryColor
                },

                Text = "Wiki",

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new WikiRoot());
                }
            });
            Add(new Button
            {
                Position = new Vector2(x, y + height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.PrimaryColor
                },

                Text = "Settings",

                OnClick = () =>
                {
                    if (TrackManager.CurrentTrack != null)
                        AddRoot(new SettingsRoot(vitaru));
                }
            });

            Add(Back = new Exit(vitaru));

            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight
            });

            Add(new Version());

            Add(new Text2D(10, true)
            {
                Y = -300,
                Text = Vitaru.ALKI > 0 ? Vitaru.ALKI == 2 ? "Rhize" : "Alki" : "Vitaru"
            });

            Renderer.Window.CursorHidden = true;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            controller.OnPrimeTrackManager = () =>
            {
                LevelPack p = LevelStore.GetRandomLevelPack(null);

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

                LevelStore.SetLevelPack(p);

                return LevelStore.CurrentLevel.Metadata;
            };

            controller.PrimeTrackManager();
        }

        protected override void OnResume()
        {
            base.OnResume();
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
                ParentOrigin = Mounts.TopRight;
                Origin = Mounts.TopRight;
                Position = new Vector2(-10, 10);
                Size = new Vector2(40);

                Background = Game.TextureStore.GetTexture("square.png");
                BackgroundSprite.Color = Color.Red;

                Text = "X";

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