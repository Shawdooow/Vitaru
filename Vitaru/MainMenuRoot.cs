// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Game;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        public MainMenuRoot()
        {
            Layers.ShaderProgram = Sprite.SpriteProgram;
            Add(new Layer2D<Sprite>
            {
                ShaderProgram = Sprite.SpriteProgram,
                ParentScaling = Axes.Both,

                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("vitaru spring 2018.png"))
                    {
                        ParentScaling = Axes.Both,
                    },
                    new Box
                    {
                        ParentScaling = Axes.Both,
                        Color = Color.Black,
                        Alpha = 0.5f,
                    }
                },
            });
        }
    }
}