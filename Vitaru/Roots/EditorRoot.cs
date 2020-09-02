// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Nucleus.Entitys;
using Vitaru.Editor.Editables;
using Vitaru.Editor.UI;
using Vitaru.Levels;

namespace Vitaru.Roots
{
    public class EditorRoot : MenuRoot
    {
        public override string Name => nameof(EditorRoot);

        protected override bool UseLevelBackground => true;

        private Editfield editfield;

        private Timeline timeline;
        private readonly LevelProperties levelProperties;
        private EditableProperties editableProperties;

        //bool to queue loading editor on draw thread, gets set by a button (the update thread)
        private LoadState state;

        public EditorRoot()
        {
            levelProperties = new LevelProperties
            {
                OnCreate = create
            };
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
                
            loadLevelEditor();

            base.LoadingComplete();
        }

        private void loadLevelEditor()
        {
            //TODO: get around this...
            Game.TextureStore.GetTexture("Edit\\enemyOutline.png");

            editfield = new Editfield();
            editableProperties = new EditableProperties();

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

            state = LoadState.Loaded;

            //This is a thicc hack, but it works so fine for now
            if (Cursor == null) return;

            //move the cursor to be on top of the editor...
            Remove(Cursor, false);
            Add(Cursor); }

        private void create()
        {
            levelProperties.FadeTo(0, 400);
            levelProperties.PassDownInput = false;

            state = LoadState.PreLoaded;
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

        public override void PreRender()
        {
            if (state == LoadState.PreLoaded)
                loadLevelEditor();

            base.PreRender();
        }
    }
}