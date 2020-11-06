// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;

namespace Vitaru.Editor.UI
{
    public class Toolbar : InputLayer<IDrawable2D>
    {
        public override string Name => nameof(Toolbar);

        private const float width = 1200;
        private const float height = 80;

        public Toolbar(LevelManager manager)
        {
            ParentOrigin = Mounts.TopCenter;
            Origin = Mounts.TopCenter;

            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Alpha = 0.8f,
                    Size = new Vector2(width, height),
                    Color = Color.Black
                },
                new Button
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterRight,

                    Width = 64,
                    Height = 24,

                    Text = "Save",

                    InstancedText =
                    {
                        TextScale = 0.3f
                    },

                    OnClick = manager.SerializeToLevel
                }
            };
        }
    }
}