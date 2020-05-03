﻿using System;
using Prion.Application.Timing;
using Prion.Game.Audio;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class Track : RepeatableSample
    {
        public readonly LevelTrack Level;

        private double nextBeat;

        public Track(LevelTrack level, ConstantClock clock) : base(level.Filename, clock)
        {
            Level = level;
        }

        public virtual bool CheckNewBeat()
        {
            if (!(Clock.LastCurrent >= nextBeat)) return false;

            nextBeat = Clock.LastCurrent + Level.GetBeatLength() / 4;
            return true;
        }
    }
}
