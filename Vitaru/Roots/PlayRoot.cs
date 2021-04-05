﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Runtime;
using Prion.Golgi.Audio.Tracks;
using Prion.Nucleus.Timing;

namespace Vitaru.Roots
{
    public class PlayRoot : MenuRoot
    {
        public override string Name => nameof(PlayRoot);

        protected override bool UseLevelBackground => true;

        protected override GCLatencyMode GCLatencyMode => GCLatencyMode.SustainedLowLatency;

        public PlayRoot()
        {
            TrackManager.SetTrackDefaults();
            TrackManager.SetPositionalDefaults();

            TrackManager.CurrentTrack.DrawClock = new SeekableClock();

            TrackManager.CurrentTrack.DrawClock.Start();
            TrackManager.CurrentTrack.DrawClock.Seek(TrackManager.CurrentTrack.Clock.Current);
            TrackManager.CurrentTrack.DrawClock.Rate = TrackManager.CurrentTrack.SeekableClock.Rate;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        public override void PreRender()
        {
            TrackManager.CurrentTrack.DrawClock.Update();
            base.PreRender();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
        }
    }
}