// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Linq;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Ribosome.Audio;
using Vitaru.Levels;
using Vitaru.Server.Levels;
using Vitaru.Settings;

namespace Vitaru.Tracks
{
    public class VitaruTrackController : TrackController
    {
        public VitaruTrackController()
        {
            AddArray(new IDrawable2D[]
            {
                new Button
                {
                    Position = new Vector2(32, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(32),
                    Background = Game.TextureStore.GetTexture("skip.png"),

                    OnClick = NextLevel,
                },
                new Button
                {
                    Position = new Vector2(-32, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(-32, 32),
                    Background = Game.TextureStore.GetTexture("skip.png"),

                    OnClick = PreviousLevel,
                },
            });
        }

        public void NextLevel()
        {
            if (TrackManager.Switching) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new("Switch to Next Level", true);

                TrackManager.PreviousTracks.Push(LevelStore.CurrentLevel.Metadata);

                if (LevelStore.NextLevelPack == null)
                    LevelStore.SetRandomLevelPack(LevelStore.CurrentPack);
                else
                {
                    LevelStore.SetLevelPack(LevelStore.NextLevelPack);
                    LevelStore.NextLevelPack = null;
                }

                TrackMetadata n = LevelStore.CurrentLevel.Metadata;
                Song.Text = $"Loading: {n.Title}";

                if (Vitaru.VitaruSettings.GetBool(VitaruSetting.RAM))
                    Game.SampleStore.ClearLoadedObjects();

                TrackManager.SetTrack(n);

                b.Finish();
            });
        }

        public void TryNextLevel()
        {
            if (TrackManager.CurrentTrack != null)
            {
                if (TrackManager.CurrentTrack.CheckFinish())
                    NextLevel();
            }
        }

        public void PreviousLevel()
        {
            if (TrackManager.Switching || !TrackManager.PreviousTracks.Any()) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new("Switch to Previous Level", true);

                TrackMetadata previous = TrackManager.PreviousTracks.Pop();
                Level l = LevelStore.GetLevel(previous);
                LevelPack p = LevelStore.GetLevelPack(l);
                LevelStore.SetLevelPack(p, l);
                Song.Text = $"Loading: {previous.Title}";

                TrackManager.SetTrack(previous);
                b.Finish();
            });
        }

        public override void PrimeTrackManager()
        {
            TrackManager.Init(Vitaru.LevelStorage);
            base.PrimeTrackManager();
        }

        protected override Texture GetBackground(string bg)
        {
            if (bg[0] == '#')
                return Vitaru.TextureStore.GetTexture(bg.Trim('#'), BackgroundFiltered);

            return bg == "default"
            ? base.GetBackground(bg)
            : Vitaru.LevelTextureStore.GetTexture(bg, BackgroundFiltered);
        }

        public override bool OnKeyDown(KeyboardKeyEvent e)
        {
            switch (e.Key)
            {
                default:
                    return base.OnKeyDown(e);
                case Keys.NextTrack:
                    NextLevel();
                    return true;
                case Keys.PreviousTrack:
                    if (TrackManager.CurrentTrack.Clock.Current < 1000)
                        PreviousLevel();
                    else
                        TrackManager.CurrentTrack.Seek(0);
                    return true;
            }
        }
    }
}