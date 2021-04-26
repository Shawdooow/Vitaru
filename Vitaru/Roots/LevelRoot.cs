// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Roots.Menu;
using Vitaru.Roots.Tests;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class LevelRoot : MenuRoot
    {
        public override string Name => nameof(LevelRoot);

        protected override bool UseLevelBackground => true;

        private readonly VitaruTrackController controller;

        public LevelRoot()
        {
            Add(new Button
            {
                ParentOrigin = Mounts.BottomRight,
                Origin = Mounts.BottomRight,
                Position = new Vector2(-10, -60),
                Size = new Vector2(100, 50),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.PrimaryColor
                },

                Text = "Play",

                OnClick = () => AddRoot(new PlayTest())
            });
            Add(new TrackSelect());
            Add(new LevelSelect());
            Add(new CharacterSelect());
            Add(new CharacterStats());
            Add(controller = new VitaruTrackController());
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.SetPositionalDefaults();
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }
    }
}