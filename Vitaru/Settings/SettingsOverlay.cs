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

        public const float WIDTH = 400;
        public const float HEIGHT = 600;

        protected bool Shown;

        private readonly Button toggle;

        public SettingsOverlay()
        {
            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Size = new Vector2(WIDTH, HEIGHT);
            Position = new Vector2(WIDTH, 0);


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
                    Spacing = 4,

                    Children = new IDrawable2D[]
                    {
                        new SpriteText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Prion",
                            TextScale = 0.24f
                        },
                        new ToggleOption<PrionSetting>(Game.Settings, PrionSetting.Fullscreen)
                        {
                            Text = "Toggle Fullscreen",
                            OnValueChange = value =>
                                Renderer.Window.WindowState = value ? WindowState.Fullscreen : WindowState.Windowed
                        },
                        new ToggleOption<PrionSetting>(Game.Settings, PrionSetting.VSync)
                        {
                            Text = "Toggle VSync"
                        },
                        new ToggleOption<PrionSetting>(Game.Settings, PrionSetting.MatchUpdate)
                        {
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
                            Text = "Toggle Particles"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ParticleCap, 4096, 65536)
                        {
                            Text = "Max Particles"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ParticleMultiplier, 0.5f,
                            2)
                        {
                            Text = "Particle Multiplier"
                        },

                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ComboFire)
                        {
                            Text = "Toggle Combo Fire"
                        },

                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ThreadBullets)
                        {
                            Text = "Multi-Thread Bullets"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.BulletCap, 512, 2048)
                        {
                            Text = "Max Bullets"
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
                this.MoveTo(new Vector2(WIDTH, 0), 200, Easings.OutCubic);
                toggle.FadeTo(1, 200, Easings.OutCubic);
                Shown = false;
            }
        }
    }
}