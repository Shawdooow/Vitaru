// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Roots.Menu;
using Vitaru.Settings;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class LevelRoot : MenuRoot
    {
        public override string Name => nameof(LevelRoot);

        protected override bool UseLevelBackground => true;

        private VitaruTrackController controller;

        public override void RenderingPreLoading()
        {
            base.RenderingPreLoading();

            Add(new Button
            {
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Position = new Vector2(-10, 10),
                Size = new Vector2(100, 50),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = ThemeManager.PrimaryColor,
                },

                Text = "Play",

                Disabled = true,
                //OnClick = () => AddRoot(new PlayRoot()),
            });
            Add(new TrackSelect());
            Add(new LevelSelect());
            Add(new CharacterSelect());
            Add(new CharacterStats());
            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight,
            });
        }

        public override void RenderingLoadingComplete()
        {
            base.RenderingLoadingComplete();
            Vitaru.VitaruSettings.SetValue(VitaruSetting.Speed, 1f);
            Add(new HacksSelect());

            Remove(Cursor, false);
            Add(Cursor);
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

            TrackManager.CurrentTrack.Pitch = Vitaru.VitaruSettings.GetFloat(VitaruSetting.Speed);
        }
    }
}