// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Settings.Options;

namespace Vitaru.Settings.Overlays
{
    public class VitaruSettingsOverlay : InputLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(VitaruSettingsOverlay);

        public const float WIDTH = 360;
        public const float HEIGHT = 640;

        public VitaruSettingsOverlay()
        {
            Size = new Vector2(WIDTH, HEIGHT);
            Position = new Vector2(WIDTH + 20, 0);

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
                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Vitaru",
                            FontScale = 0.36f
                        },
                        new Text2D
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
                            4)
                        {
                            Text = "Particle Multiplier"
                        },

                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.ComboFire)
                        {
                            Text = "Toggle Combo Fire"
                        },

                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Multithreading)
                        {
                            Text = "Additional Multi-threading"
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.BulletCap, 512, 2048)
                        {
                            Text = "Max Bullets"
                        },

                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Debug",
                            FontScale = 0.24f
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.DebugHacks)
                        {
                            Text = "Toggle \"GOD-KING\" Hacks"
                        }
                    }
                }
            };
        }
    }
}