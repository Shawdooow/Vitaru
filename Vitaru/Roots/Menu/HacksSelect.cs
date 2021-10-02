﻿using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Settings;
using Vitaru.Settings.Options;

namespace Vitaru.Roots.Menu
{
    public class HacksSelect : InputLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(HacksSelect);

        public const float WIDTH = 240;
        public const float HEIGHT = 320;

        public HacksSelect()
        {
            Size = new Vector2(WIDTH, HEIGHT);
            Position = new Vector2(-WIDTH - 160, 0);

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    Size = Size,
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
                            FontScale = 0.36f,
                        },
                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Gameplay",
                            FontScale = 0.24f,
                        },
                        new SliderOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Speed, 0.5f, 2f)
                        {
                            Text = "Speed Hacks",
                        },

                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Debug",
                            FontScale = 0.24f,
                        },
                        new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.DebugHacks)
                        {
                            Text = "Toggle \"GOD-KING\" Hacks",
                        },
                    },
                },
            };
        }
    }
}