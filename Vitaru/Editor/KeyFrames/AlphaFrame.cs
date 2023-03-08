// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class AlphaFrame : FloatFrame
    {
        private readonly IHasAlpha i;

        public AlphaFrame(IHasAlpha i)
        {
            this.i = i;
        }

        public override void ApplyFrame(double current, KeyFrame next)
        {
            if (next == null)
            {
                i.Alpha = Value;
                return;
            }

            AlphaFrame n = (AlphaFrame)next;

            double adjusted = PrionMath.Remap(current, StartTime, next.StartTime);
            double ease = Prion.Nucleus.Utilities.Easing.ApplyEasing(Easing, adjusted);
            i.Alpha = (float)PrionMath.Remap(ease, 0, 1, Value, n.Value);
        }
    }
}
