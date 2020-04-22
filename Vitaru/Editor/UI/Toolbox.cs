// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Editor.UI
{
    public class Toolbox : Layer2D<IDrawable2D>
    {
        public override string Name => "Toolbox";

        public Toolbox()
        {
            ParentOrigin = Mounts.CenterLeft;
            Origin = Mounts.CenterLeft;

            Position = new Vector2(10, 0);
            Size = new Vector2(200, 500);

            Children = new IDrawable2D[]
            {
                new SpriteLayer
                {
                    Name = "Background",
                    Child = new Box
                    {
                        Alpha = 0.8f,
                        Size = new Vector2(200, 500),
                        Color = Color.Black,
                    },
                },
            };
        }
    }
}