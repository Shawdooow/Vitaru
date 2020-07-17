// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Timing;
using Vitaru.Levels;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public static class TrackManager
    {
        public static Track CurrentTrack { get; private set; }

        public static Action<Track> OnTrackChange;

        public static void SetTrack(LevelTrack level, SeekableClock clock = null)
        {
            SeekableClock seek = clock ?? CurrentTrack.Clock;

            seek.Stop();
            seek.Reset();

            CurrentTrack?.Pause();
            CurrentTrack?.Dispose();

            Logger.Log($"Setting Track \"{level.Name}\"");
            CurrentTrack = new Track(level, seek, Vitaru.LevelStorage.GetStorage($"{level.Name}"));
            OnTrackChange?.Invoke(CurrentTrack);

            seek.Start();
            CurrentTrack.Play();
        }

        public static void NextTrack()
        {
            LevelTrack next = LevelStore.GetRandomLevel(CurrentTrack.Level);
            SetTrack(next, CurrentTrack.Clock);
        }

        public static void TryNextTrack()
        {
            if (CurrentTrack.CheckFinish())
                NextTrack();
        }

        public static void TryRepeatTrack()
        {
            CurrentTrack.TryRepeat();
        }
    }
}