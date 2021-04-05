using Prion.Golgi.Audio.Tracks;
using Prion.Ribosome.Audio;
using Vitaru.Levels;

namespace Vitaru.Tracks
{
    public static class TrackManagerExtentions
    {
        public static void NextTrack()
        {
            TrackMetadata next = LevelStore.SetRandomLevel(LevelStore.CurrentPack);
            TrackManager.SetTrack(next, TrackManager.CurrentTrack.SeekableClock);
        }

        public static void TryNextTrack()
        {
            if (TrackManager.CurrentTrack.CheckFinish())
                NextTrack();
        }
    }
}
