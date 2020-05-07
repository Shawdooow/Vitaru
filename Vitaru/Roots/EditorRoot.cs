// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Vitaru.Editor.UI;

namespace Vitaru.Roots
{
    public class EditorRoot : MenuRoot
    {
        private readonly EditableGamefield gamefield;

        public EditorRoot()
        {
            gamefield = new EditableGamefield();

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Box
                    {
                        Name = "Gamefield BG",
                        Color = Color.Black,
                        Alpha = 0.8f,
                        Size = new Vector2(1024, 820),
                        Scale = new Vector2(0.5f)
                    }
                }
            });

            //Packs
            Add(gamefield);

            //Layers
            Add(gamefield.ProjectileLayer);
            Add(gamefield.CharacterLayer);

            Add(new Timeline());
            Add(new Toolbox());
        }
    }
}