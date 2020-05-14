﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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

        public override void Play()
        {
            base.Play();
        }

        public override void Seek(double time)
        {
            base.Seek(time);
        }

        public virtual bool CheckNewBeat()
        {
            if (!(Clock.LastCurrent >= nextBeat)) return false;

            nextBeat = Clock.LastCurrent + Level.GetBeatLength();
            return true;
        }

        public static LevelTrack GetBells() => new LevelTrack
        {
            Name = "Alki Bells",
            Filename = "alki bells.mp3",
            Artist = "Shawdooow",
            BPM = 96
        };

        public static LevelTrack GetEndgame() => new LevelTrack
        {
            Name = "Alki Endgame",
            Filename = "alki endgame.wav",
            Artist = "Shawdooow",
            BPM = 96
        };
    }
}