// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input.Events;
using Vitaru.Roots.Menu;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class MainMenu : MenuRoot
    {
        public override string Name => nameof(MainMenu);

        protected override bool Parallax => true;

        private readonly TrackController controller;

        public MainMenu(Vitaru vitaru)
        {
            Back = new Button();

            Add(new MainMenuPanel());

            Add(new InstancedText
            {
                Position = new Vector2(-40, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Text = Vitaru.ALKI > 0 ? Vitaru.ALKI == 2 ? "Rhize" : "Alki" : "Vitaru",
                FontScale = 1.5f
            });

            Add(controller = new TrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight
            });
            Add(new Version());

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

        private class MainMenuPanel : InputLayer<IDrawable2D>
        {
            private InputLayer<IDrawable2D> header;
            private Box bar;
            private InputLayer<IDrawable2D> body;

            public MainMenuPanel()
            {
                Position = new Vector2(40);
                ParentOrigin = Mounts.TopLeft;
                Origin = Mounts.TopLeft;

                Size = new Vector2(64 * 8, 64);

                Children = new IDrawable2D[]
                {
                    header = new InputLayer<IDrawable2D>
                    {
                        Size = new Vector2(64 * 8, 64),

                        Children = new IDrawable2D[]
                        {
                            new Box
                            {
                                Size = Size,
                                Color = Color.White
                            },
                            new MenuButton
                            {
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("solo.png"),

                                HoverSprite =
                                {
                                    Color = ThemeManager.PrimaryColor
                                },

                                OnClick = () => select(Buttons.Solo)
                            },
                            new MenuButton
                            {
                                X = 64,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("multi.png"),

                                HoverSprite =
                                {
                                    Color = ThemeManager.SecondaryColor
                                },

                                OnClick = () => select(Buttons.Multi)
                            },
                            new MenuButton
                            {
                                X = 128,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("edit.png"),

                                HoverSprite =
                                {
                                    Color = ThemeManager.TrinaryColor
                                },

                                OnClick = () => select(Buttons.Edit)
                            },
                            new MenuButton
                            {
                                X = 128 + 64,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("wiki.png"),

                                HoverSprite =
                                {
                                    Color = ThemeManager.QuadnaryColor
                                },

                                OnClick = () => select(Buttons.Wiki)
                            },
                            new MenuButton
                            {
                                X = 256,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("mods.png"),

                                HoverSprite =
                                {
                                    Color = Color.DarkOrange
                                },

                                OnClick = () => select(Buttons.Mods)
                            },
                            new MenuButton
                            {
                                X = 256 + 64,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("star.png"),

                                HoverSprite =
                                {
                                    Color = Color.LightSeaGreen
                                },

                                OnClick = () => select(Buttons.Settings)
                            }
                        }
                    },
                    bar = new Box
                    {
                        Y = 64,
                        ParentOrigin = Mounts.TopLeft,
                        Origin = Mounts.TopLeft,
                        Size = new Vector2(Width, 10),
                        Color = Color.White
                    },
                    body = new InputLayer<IDrawable2D>()
                };

                bar.Color = ThemeManager.PrimaryColor;
            }

            private void select(Buttons button)
            {
                foreach (IDrawable2D child in header)
                    if (child is MenuButton b && b.HoverSprite.Alpha == 1)
                    {
                        b.HoverSprite.FadeTo(0, 200);
                        break;
                    }

                switch (button)
                {
                    case Buttons.Solo:
                        bar.ColorTo(ThemeManager.PrimaryColor, 200);
                        break;
                    case Buttons.Multi:
                        bar.ColorTo(ThemeManager.SecondaryColor, 200);
                        break;
                    case Buttons.Edit:
                        bar.ColorTo(ThemeManager.TrinaryColor, 200);
                        break;
                    case Buttons.Wiki:
                        bar.ColorTo(ThemeManager.QuadnaryColor, 200);
                        break;
                    case Buttons.Mods:
                        bar.ColorTo(Color.DarkOrange, 200);
                        break;
                    case Buttons.Settings:
                        bar.ColorTo(Color.LightSeaGreen, 200);
                        break;
                    case Buttons.Quit:
                        bar.ColorTo(Color.Red, 200);
                        break;
                }
            }

            private enum Buttons
            {
                Solo,
                Multi,
                Edit,
                Wiki,
                Mods,
                Settings,

                Quit
            }

            private class MenuButton : Button
            {
                public bool Focused { get; private set; }

                public MenuButton()
                {
                    //Just moves it to render last (or on top of everything else)
                    Remove(BackgroundSprite, false);
                    Add(BackgroundSprite);
                }

                public override bool OnMouseDown(MouseButtonEvent e)
                {
                    if (base.OnMouseDown(e))
                        return Focused = true;

                    return Focused = false;
                }

                public override void OnHovered()
                {
                    if (HoverSprite.Alpha != 1)
                        base.OnHovered();
                    else
                        Hovered = true;
                }

                public override void OnHoverLost()
                {
                    if (HoverSprite.Alpha != 1)
                        base.OnHoverLost();
                    else
                        Hovered = false;
                }

                protected override void Flash()
                {
                    HoverSprite.Alpha = 1;
                }
            }
        }
    }
}