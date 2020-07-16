using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UserInterface;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Levels;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class TrackController : InputLayer<IDrawable2D>
    {
        private readonly Button next;
        private readonly SpriteText song;

        private bool qued;
        private string bg = string.Empty;

        public TrackController()
        {
            ParentOrigin = Mounts.TopRight;
            Origin = Mounts.TopRight;

            Add(next = new Button
            {
                Position = new Vector2(-10, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Size = new Vector2(160, 90),

                Dim =
                {
                    Alpha = 0.8f
                },
                SpriteText =
                {
                    TextScale = 0.25f
                },

                Text = "Next",

                OnClick = NextLevel
            });

            Add(song = new SpriteText
            {
                Position = new Vector2(-10, 10),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f,
                Text = "Loading..."
            });
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            enter();
        }

        //Having a drawable and updatable is bad practice, however here it just makes sense because its a UI element
        public void Update()
        {
            TrackManager.SeekableClock?.NewFrame();
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty)
            {
                next.Background = Vitaru.LevelTextureStore.GetTexture(bg);
                bg = string.Empty;
            }
        }

        public void PrimeTrackManager()
        {
            qued = true;
            Game.ScheduleLoad(() =>
            {
                Benchmark track = new Benchmark("Prime TrackManager", true);

                LevelTrack t = LevelStore.LoadedLevels[PrionMath.RandomNumber(0, LevelStore.LoadedLevels.Count)]
                    .Levels[0]
                    .LevelTrack;
                song.Text = $"Loading: {t.Name}";
                TrackManager.SetTrack(t, new SeekableClock());

                track.Finish();

                qued = false;
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

        public void NextLevel()
        {
            if (qued) return;
            qued = true;

            Game.ScheduleLoad(() =>
            {
                Benchmark b = new Benchmark("Switch Level", true);

                LevelTrack n = LevelStore.GetRandomLevel(TrackManager.CurrentTrack.Level);
                song.Text = $"Loading: {n.Name}";

                TrackManager.SetTrack(n); b.Finish();

                qued = false;
            });
        }

        private void change(Track t) 
        {
            song.Text = $"Now Playing: {t.Level.Name}";

            if (t.Level.Image != string.Empty)
                bg = $"{t.Level.Name}\\{t.Level.Image}";
        }
        private void enter() => TrackManager.OnTrackChange += change;
        private void leave() => TrackManager.OnTrackChange -= change;

        public override void OnResume()
        {
            base.OnResume();
            enter();
        }

        public override void OnPause()
        {
            base.OnPause();
            leave();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            leave();
        }
    }
}
