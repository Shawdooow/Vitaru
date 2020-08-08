﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using System.Drawing;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input.Events;
using Vitaru.Themes;

namespace Vitaru.Roots
{
    public class MainMenuRoot : MenuRoot
    {
        public override string Name => nameof(MainMenuRoot);

        public MainMenuRoot(Vitaru vitaru)
        {
            Back = new Button();

            Add(new MainMenuPanel());

            Add(new SpriteText
            {
                Position = new Vector2(-90, 90),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru",
                TextScale = 1.5f
            });
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
                Position = new Vector2(100);
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
                                X = 192,
                                ParentOrigin = Mounts.TopLeft,
                                Origin = Mounts.TopLeft,
                                Size = new Vector2(64),
                                Background = Vitaru.TextureStore.GetTexture("pause.png"),

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
                                Background = Vitaru.TextureStore.GetTexture("skip.png"),

                                HoverSprite =
                                {
                                    Color = Color.DarkOrange
                                },

                                OnClick = () => select(Buttons.Mods)
                            },
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
                    body = new InputLayer<IDrawable2D>
                    {

                    }
                };

                select(Buttons.Solo);
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
                        bar.Color = ThemeManager.PrimaryColor;
                        break;
                    case Buttons.Multi:
                        bar.Color = ThemeManager.SecondaryColor;
                        break;
                    case Buttons.Edit:
                        bar.Color = ThemeManager.TrinaryColor;
                        break;
                    case Buttons.Wiki:
                        bar.Color = ThemeManager.QuadnaryColor;
                        break;
                    case Buttons.Mods:
                        bar.Color = Color.DarkOrange;
                        break;
                    case Buttons.Quit:
                        bar.Color = Color.Red;
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

                Quit,
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