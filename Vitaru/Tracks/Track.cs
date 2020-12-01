// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Audio.Contexts;
using Prion.Nucleus.Timing;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class Track : Song
    {
        public readonly LevelTrack Level;

        private double nextBeat;

        public new readonly SeekableClock Clock;

        public SeekableClock DrawClock { get; set; }

        public override float Pitch
        {
            get => base.Pitch;
            set
            {
                base.Pitch = value;
                Clock.Rate = value;
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

            time *= 1000;
            Clock.Seek(time);
            DrawClock?.Seek(time);
        }

        public virtual bool CheckNewBeat()
        {
            if (!(Clock.LastCurrent >= nextBeat)) return false;

            nextBeat = Clock.LastCurrent + Level.GetBeatLength();
            return true;
        }
    }
}