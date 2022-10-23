// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Timing;
using Vitaru.Editor.UI;
using Vitaru.Levels;
using Vitaru.Server.Levels;

namespace Vitaru.Roots
{
    public class EditorRoot : MenuRoot
    {
        public override string Name => nameof(EditorRoot);

        protected override bool UseLevelBackground => true;

        private Timeline timeline;
        private readonly LevelProperties levelProperties;

        //state to manage loading the editor on draw thread, gets set by a button (the update thread)
        private LoadState state;

        public EditorRoot()
        {
            TrackManager.SetTrackDefaults();
            TrackManager.SetPositionalDefaults();

            levelProperties = new LevelProperties
            {
                OnCreate = create,
            };
        }

        public override void LoadingComplete()
        {
            //TODO: get around these...
            Game.TextureStore.GetTexture("Gameplay\\enemy.png");
            Game.TextureStore.GetTexture("Edit\\enemyOutline.png");

            Add(levelProperties);

            if (LevelStore.CurrentPack.Levels[0].Format == LevelStore.BLANK)
            {
                base.LoadingComplete();

                levelProperties.Alpha = 1;
                levelProperties.PassDownInput = true;
                return;
            }

            loadLevelEditor(LevelStore.CurrentLevel);

            base.LoadingComplete();
        }

        private void loadLevelEditor(Level level)
        {
            TrackManager.CurrentTrack.DrawClock = new SeekableClock();

            TrackManager.CurrentTrack.DrawClock.Start();
            TrackManager.CurrentTrack.DrawClock.Seek(TrackManager.CurrentTrack.Clock.Current);
            TrackManager.CurrentTrack.DrawClock.Rate = TrackManager.CurrentTrack.SeekableClock.Rate;

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
                        Scale = new Vector2(0.5f),
                    },
                },
            });

            Add(new Toolbar());
            Add(timeline = new Timeline());

            state = LoadState.Loaded;

            //This is a thicc hack, but it works so its fine for now
            if (Cursor == null) return;

            //move the cursor to be on top of the editor...
            Remove(Cursor, false);
            Add(Cursor);
        }

        private void create()
        {
            levelProperties.FadeTo(0, 400);
            levelProperties.PassDownInput = false;

            state = LoadState.PreLoaded;
        }

        public override void Update()
        {
            base.Update();
            timeline?.Update();
        }

        public override void PreRender()
        {
            TrackManager.CurrentTrack.DrawClock?.Update();

            if (state == LoadState.PreLoaded)
                loadLevelEditor(LevelStore.CurrentLevel);

            base.PreRender();
        }
    }
}