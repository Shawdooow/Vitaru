// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
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
        private readonly LevelProperties levelProperties;
        private readonly EditableProperties editableProperties;

        public EditorRoot()
        {
            levelProperties = new LevelProperties();
            if (LevelStore.CurrentPack.Levels[0].Format == LevelStore.BLANK_LEVEL) return;

            editfield = new Editfield();
            editableProperties = new EditableProperties();
        }

        public override void LoadingComplete()
        {
            Add(levelProperties);

            if (LevelStore.CurrentPack.Levels[0].Format == LevelStore.BLANK_LEVEL)
            {
                levelProperties.Alpha = 1;
                levelProperties.PassDownInput = true;
                base.LoadingComplete();
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

            Add(new Toolbar());
            Add(timeline = new Timeline());
            Add(new Toolbox
            {
                OnSelection = Selected
            });
            Add(editableProperties);

            base.LoadingComplete();
        }

        protected void Selected(Editable editable)
        {
            editfield.Selected(editable);
            editableProperties.Selected(editable);
        }

        public override void Update()
        {
            base.Update();
            timeline?.Update();
        }
    }
}