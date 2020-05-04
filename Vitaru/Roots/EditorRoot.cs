// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;
using Vitaru.Editor.UI;

namespace Vitaru.Roots
{
    public class EditorRoot : Root
    {
        private readonly EditableGamefield gamefield;
        private readonly Box shade;

        public EditorRoot()
        {
            gamefield = new EditableGamefield();

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    shade = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    },
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

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            shade.FadeTo(0.8f, 1000);
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