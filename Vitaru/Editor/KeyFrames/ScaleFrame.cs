using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class ScaleFrame : Vector2Frame
    {
        private readonly IHasScale i;

        public ScaleFrame(IHasScale i)
        {
            this.i = i;
        }

        public override void ApplyFrame(double current, KeyFrame next)
        {
            if (next == null)
            {
                i.Scale = Value;
                return;
            }

            ScaleFrame n = (ScaleFrame)next;

            double adjusted = PrionMath.Remap(current, StartTime, next.StartTime);
            double ease = Prion.Nucleus.Utilities.Easing.ApplyEasing(Easing, adjusted);
            i.Scale = PrionMath.Remap(ease, 0, 1, Value, n.Value);
        }
    }
}
