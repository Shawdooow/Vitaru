// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Core.IO;
using Prion.Core.Timing;
using Prion.Game.Audio;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class Track : RepeatableSample
    {
        public readonly LevelTrack Level;

        private double nextBeat;

        public Track(LevelTrack level, ConstantClock clock, Storage storage = null) : base(level.Filename, clock, storage)
        {
            Level = level;
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