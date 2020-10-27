// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.IO.Configs;
using Prion.Nucleus.Utilities;
using Vitaru.Settings.Options;

namespace Vitaru.Settings
{
    public class SettingsOverlay : HoverableLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(SettingsOverlay);

        private const float width = 400;
        private const float height = 600;

        protected bool Shown;

        private Button toggle;

        public SettingsOverlay()
        {
            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Size = new Vector2(width, height);
            Position = new Vector2(width, 0);


            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    Size = Size
                },

                new ListLayer<IDrawable2D>
                {
                    Size = Size,
                    Spacing = 2,

                    Children = new IDrawable2D[]
                    {
                        new SpriteText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Prion",
                            TextScale = 0.24f
                        }, 
                        new ToggleOption<PrionSetting>(Game.PrionSettings, PrionSetting.Fullscreen)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle Fullscreen",
                            OnValueChange = value => Renderer.Window.WindowState = value ? WindowState.Fullscreen : WindowState.Windowed
                        },
                        new ToggleOption<PrionSetting>(Game.PrionSettings, PrionSetting.VSync)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle VSync"
                        },
                        new ToggleOption<PrionSetting>(Game.PrionSettings, PrionSetting.MatchUpdate)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Limit Draw to Update",
                            OnValueChange = value => Renderer.MatchUpdateRate = value
                        },

                        new SpriteText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Gameplay",
                            TextScale = 0.24f
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Touch)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle Touch Mode"
                        },

                        new SpriteText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Graphics",
                            TextScale = 0.24f
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Particles)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle Particles"
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ComboFire)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle Combo Fire"
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ThreadBullets)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Multi-Thread Bullets"
                        },

                        new SpriteText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Debug",
                            TextScale = 0.24f
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.DebugHacks)
                        {
                            Size = new Vector2(width - 8, 20),
                            Text = "Toggle \"GOD-KING\" Hacks"
                        }
                    }
                },

                toggle = new Button
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,

                    Size = new Vector2(100, 200),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Text = "Settings",
                    SpriteText =
                    {
                        TextScale = 0.25f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.DarkSlateBlue
                    },

                    OnClick = () =>
                    {
                        if (toggle.Alpha > 0)
                            Toggle();
                    }
                }
            };
        }

        public override bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButtons.Left && !Hovered && toggle.Alpha < 1)
                Hide();
            return base.OnMouseDown(e);
        }

        public void Toggle()
        {
            if (Shown)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (!Shown)
            {
                this.MoveTo(Vector2.Zero, 200, Easings.OutCubic);
                toggle.FadeTo(0, 200, Easings.OutCubic);
                Shown = true;
            }
        }

        public void Hide()
        {
            if (Shown)
            {
                this.MoveTo(new Vector2(width, 0), 200, Easings.OutCubic);
                toggle.FadeTo(1, 200, Easings.OutCubic);
                Shown = false;
            }
        }
    }
}