﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Audio;
using Prion.Nucleus.IO;
using Prion.Nucleus.Timing;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class Track : RepeatableSample
    {
        public readonly LevelTrack Level;

        private double nextBeat;

        public new readonly SeekableClock Clock;

        public override float Pitch
        {
            get => base.Pitch;
            set
            {
                base.Pitch = value;
                Clock.Rate = value;
            }
        }

        public Track(LevelTrack level, SeekableClock clock, Storage storage) : base(level.Filename, clock,
            storage)
        {
            Clock = clock;
            Level = level;
        }

        public override void Seek(double time)
        {
            base.Seek(time);
            Clock.Seek(time * 1000);
        }

        public virtual bool CheckNewBeat()
        {
            if (!(Clock.LastCurrent >= nextBeat)) return false;

            nextBeat = Clock.LastCurrent + Level.GetBeatLength();
            return true;
        }
    }
}