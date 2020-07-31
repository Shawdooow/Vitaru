// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Editor.Editables;
using Vitaru.Editor.UI;
using Vitaru.Levels;

namespace Vitaru.Roots
{
    public class EditorRoot : MenuRoot
    {
        public override string Name => nameof(EditorRoot);

        protected override bool UseLevelBackground => true;

        private readonly Editfield editfield;

        private Timeline timeline;
        private readonly Properties properties;

        public EditorRoot()
        {
            if (LevelStore.CurrentPack.Levels[0].Format == LevelStore.BLANK_LEVEL) return;

            editfield = new Editfield();
            properties = new Properties();
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            if (LevelStore.CurrentPack.Levels[0].Format == LevelStore.BLANK_LEVEL)
            {
                Add(new SpriteText
                {
                    Text = "NO LEVEL DATA!"
                });
                return;
            }

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

            Add(timeline = new Timeline());
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

        public override void Update()
        {
            base.Update();
            timeline?.Update();
        }
    }
}