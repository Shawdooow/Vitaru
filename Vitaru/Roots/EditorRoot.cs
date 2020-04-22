// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Editor.UI;

namespace Vitaru.Roots
{
    public class EditorRoot : Root
    {
        public EditorRoot()
        {
            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("Backgrounds\\vitaru spring 2018.png"))
                    {
                        Scale = new Vector2(0.75f),
                    },
                    new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5),
                    },
                },
            });

            Add(new Toolbox());
        }
    }
}