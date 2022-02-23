using System;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class PathFrame : Vector2Frame
    {
        public CurveType CurveType { get; set; }

        public float CurveAmount { get; set; }

        private readonly IHasPosition i;

        public PathFrame(IHasPosition i)
        {
            this.i = i;
        }

        public override void ApplyFrame(double current, KeyFrame next)
        {
            PathFrame n = (PathFrame)next;

            if (next == null)
            {
                i.Position = Value;
                return;
            }

            double scale = Math.Clamp(PrionMath.Remap(current, StartTime, n.StartTime), 0, 1);
            float t = (float)Prion.Nucleus.Utilities.Easing.ApplyEasing(Easing, scale);

            switch (CurveType)
            {
                default:
                    i.Position = new Vector2(PrionMath.Remap(t, 0, 1, Value.X, n.Value.X),
                        PrionMath.Remap(t, 0, 1, Value.Y, n.Value.Y));
                    break;
                case CurveType.Bezier:
                    float angle = MathF.Atan2(n.Value.Y - Value.Y,
                        n.Value.X - Value.X);

                    //Half way to the EndPoint
                    Vector2 point = Value + PrionMath.Offset(Vector2.Distance(Value, n.Value) / 2, angle) +
                                    //Then add the "CurveAmount" to slide it to one side or the other
                                    PrionMath.Offset(CurveAmount, angle - MathF.PI / 2f);

                    i.Position = PrionMath.Bezier(t, Value, point, n.Value);
                    break;
            }
        }
    }

    public enum CurveType
    {
        Straight,
        Target,

        Bezier,
    }
}
