using Prion.Application.Timing;

namespace Vitaru.Editor
{
    public class SeekableClock : AtomicClock
    {
        public override double Current => base.Current - seekOffset;

        public override double LastCurrent => base.LastCurrent - seekOffset;

        private double seekOffset;

        public void Seek(double time) => seekOffset = base.Current - time;
    }
}
