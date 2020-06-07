// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Editor.UI
{
    public class Timeline : InputLayer<IDrawable2D>
    {
        private const float width = 1080f;
        private const float height = 140f;

        public Timeline()
        {
            ParentOrigin = Mounts.BottomCenter;
            Origin = Mounts.BottomCenter;

            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new SpriteLayer
                {
                    Name = "Background",
                    Child = new Box
                    {
                        Alpha = 0.8f,
                        Size = new Vector2(width, height),
                        Color = Color.Black
                    }
                }
            };
        }
    }
}