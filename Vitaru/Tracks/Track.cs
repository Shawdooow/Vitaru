﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Audio.Contexts;
using Prion.Nucleus.Timing;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class Track : Sound
    {
        public readonly LevelTrack Level;

        private double nextBeat;

        public SeekableClock DrawClock { get; set; }

        public override float Pitch
        {
            get => base.Pitch;
            set
            {
                base.Pitch = value;
                SeekableClock.Rate = value;
                if (DrawClock != null) DrawClock.Rate = value;
            }
        }

        public Track(LevelTrack level, SeekableClock clock, Sample sample) : base(clock, sample)
        {
            Clock = clock;
            Level = level;
        }

        public override void Seek(double time)
        {
            base.Seek(time);
            DrawClock?.Seek(time * 1000);
        }

        public virtual bool CheckNewBeat()
        {
            if (!(Clock.LastCurrent >= nextBeat)) return false;

            nextBeat = Clock.LastCurrent + Level.GetBeatLength();
            return true;
        }
    }
}