﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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