// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;

namespace Vitaru.Editor.UI
{
    public class Toolbar : InputLayer<IDrawable2D>
    {
        public override string Name => nameof(Toolbar);

        private const float width = 1280;
        private const float height = 120;

        public Toolbar()
        {
            ParentOrigin = Mounts.BottomCenter;
            Origin = Mounts.BottomCenter;

            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Alpha = 0.8f,
                    Size = new Vector2(width, height),
                    Color = Color.Black,
                },
                new Button
                {
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.TopRight,

                    Width = 72,
                    Height = 32,

                    Text = "Save",
                    Disabled = true,

                    Text2D =
                    {
                        FontScale = 0.35f,
                    },
                },
            };
        }
    }
}