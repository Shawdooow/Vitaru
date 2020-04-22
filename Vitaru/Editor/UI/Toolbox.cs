// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Editor.UI
{
    public class Toolbox : Layer2D<IDrawable2D>
    {
        public Toolbox()
        {
            ParentOrigin = Mounts.CenterLeft;
            Origin = Mounts.CenterLeft;

            Children = new IDrawable2D[]
            {
                new SpriteLayer
                {
                    Child = new Box
                    {
                        Size = new Vector2(200, 500),
                    },
                },
            };
        }
    }
}