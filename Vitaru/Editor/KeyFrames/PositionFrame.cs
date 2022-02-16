using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class PositionFrame : Vector2Frame
    {
        private readonly IHasPosition i;

        public PositionFrame(IHasPosition i)
        {
            this.i = i;
        }

        public override void ApplyFrame(double current, KeyFrame next)
        {
            if (next == null)
            {
                i.Position = Value;
                return;
            }

            PositionFrame n = (PositionFrame)next;
            i.Position = PrionMath.Remap(current, Time, next.Time, Value, n.Value);
        }
    }
}
