// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Roots
{
    public abstract class MenuRoot : Root
    {
        protected readonly Sprite Background;
        protected readonly Box Shade;

        protected MenuRoot()
        {
            Add(new SpriteLayer
            {
                Children = new[]
                {
                    Background = new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    Shade = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    }
                }
            });
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Escape:
                    DropRoot();
                    break;
            }
        }
    }
}