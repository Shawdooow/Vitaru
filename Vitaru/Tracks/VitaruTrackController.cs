using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Ribosome.Audio;
using Vitaru.Levels;

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

                    OnClick = NextLevel
                },
                new Button
                {
                    Position = new Vector2(-32, -8),
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(-32, 32),
                    Background = Game.TextureStore.GetTexture("skip.png"),

                    OnClick = PreviousLevel
                }
            });
        }

        public void NextLevel()
        {
            if (TrackManager.Switching) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new("Switch to Next Level", true);

                TrackManager.PreviousLevels.Push(LevelStore.CurrentLevel.Metadata);

                TrackMetadata n = LevelStore.SetRandomLevel(LevelStore.CurrentPack);
                Song.Text = $"Loading: {n.Title}";

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
            if (TrackManager.Switching || !TrackManager.PreviousLevels.Any()) return;

            TrackManager.Switching = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new("Switch to Previous Level", true);

                TrackMetadata previous = TrackManager.PreviousLevels.Pop();
                LevelStore.SetLevel(LevelStore.GetLevelPack(previous));
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
            return bg == "default" ? base.GetBackground(bg) : Vitaru.LevelTextureStore.GetTexture(bg);
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
                    PreviousLevel();
                    return true;
            }
        }
    }
}
