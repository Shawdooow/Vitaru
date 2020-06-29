// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Vitaru.Editor.IO;
using Vitaru.Editor.UI;

namespace Vitaru.Roots
{
    public class EditorRoot : MenuRoot
    {
        public override string Name => nameof(EditorRoot);

        private readonly Editfield editfield;

        private readonly Properties properties;

        public EditorRoot()
        {
            editfield = new Editfield();
            properties = new Properties();
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Game.TextureStore.GetTexture("Edit\\enemyOutline.png");

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
            Add(editfield);

            //Layers
            Add(editfield.CharacterLayer);
            Add(editfield.ProjectilesLayer);
            Add(editfield.SelectionLayer);

            Add(new Timeline());
            Add(new Toolbox
            {
                OnSelection = Selected
            });
            Add(properties);
        }

        protected void Selected(Editable editable)
        {
            editfield.Selected(editable);
            properties.Selected(editable);
        }
    }
}