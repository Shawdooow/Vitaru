// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
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

        public const float WIDTH = 360;
        public const float HEIGHT = 640;

        protected bool Shown;

        private readonly Button toggle;

        public SettingsOverlay(Game game)
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
                        new InstancedText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Prion",
                            FontScale = 0.24f
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
                        new SliderOption<PrionSetting>(Game.Settings, PrionSetting.UpdateFrequency, 30, 1000)
                        {
                            Text = "Update Frequency",
                            OnValueChange = value => game.UpdateFrequency = (int) value
                        },
                        new SliderOption<PrionSetting>(Game.Settings, PrionSetting.DrawFrequency, 30, 1000)
                        {
                            Text = "Draw Frequency",
                            OnValueChange = value => Renderer.DrawFrequency = (int) value
                        },
                        new SliderOption<PrionSetting>(Game.Settings, PrionSetting.IdleUpdate, 10, 60)
                        {
                            Text = "Idle Update Frequency",
                            OnValueChange = value => game.IdleUpdate = (int) value
                        },
                        new SliderOption<PrionSetting>(Game.Settings, PrionSetting.IdleDraw, 10, 60)
                        {
                            Text = "Idle Draw Frequency",
                            OnValueChange = value => Renderer.IdleDraw = (int) value
                        },
                        new ToggleOption<PrionSetting>(Game.Settings, PrionSetting.LimitDrawToUpdate)
                        {
                            Text = "Limit Draw to Update",
                            OnValueChange = value => Renderer.LimitDrawToUpdate = value
                        },

                        //new InstancedText
                        //{
                        //    ParentOrigin = Mounts.TopCenter,
                        //    Origin = Mounts.TopCenter,
                        //    Text = "Gameplay",
                        //    TextScale = 0.24f
                        //},
                        //new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Touch)
                        //{
                        //    Text = "Toggle Touch Mode"
                        //},

                        new InstancedText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Graphics",
                            FontScale = 0.24f
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

                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Multithreading)
                        {
                            Text = "Additional Multithreading"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.BulletCap, 512, 2048)
                        {
                            Text = "Max Bullets"
                        },

                        new InstancedText
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Debug",
                            FontScale = 0.24f
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.DebugHacks)
                        {
                            Text = "Toggle \"GOD-KING\" Hacks"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.EnemyMultiplier, 1, 4)
                        {
                            Text = "Enemy Multiplier"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.BulletMultiplier, 1, 4)
                        {
                            Text = "Bullet Multiplier"
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
                    InstancedText =
                    {
                        FontScale = 0.25f
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