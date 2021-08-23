// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus;
using Prion.Nucleus.Settings;
using Vitaru.Settings.Options;

namespace Vitaru.Settings.Overlays
{
    public class NucleusSettingsOverlay : InputLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(NucleusSettingsOverlay);

        public NucleusSettingsOverlay(Application application)
        {
            Size = new Vector2(VitaruSettingsOverlay.WIDTH, VitaruSettingsOverlay.HEIGHT);
            Position = new Vector2(-VitaruSettingsOverlay.WIDTH - 20, 0);

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
                            Text = "Nucleus",
                            FontScale = 0.36f
                        },
                        //new Text2D
                        //{
                        //    ParentOrigin = Mounts.TopCenter,
                        //    Origin = Mounts.TopCenter,
                        //    Text = "Graphics",
                        //    FontScale = 0.24f
                        //},
                        new SliderOption<NucleusSetting>(Application.Settings, NucleusSetting.UpdateFrequency, 30, 1000)
                        {
                            Text = "Update Frequency",
                            OnValueChange = value => application.UpdateFrequency = (int) value
                        }
                    }
                }
            };
        }
    }
}