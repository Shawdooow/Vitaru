// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Timing;

namespace Vitaru.Editor
{
    public class SeekableClock : AdjustableClock
    {
        public override double Current => base.Current - seekOffset;

        public override double LastCurrent => base.LastCurrent - seekOffset;

        private double seekOffset;

        public void Seek(double time) => seekOffset = base.Current - time;
    }
}