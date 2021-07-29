using System.Numerics;

namespace Vitaru.Play
{
    public interface Hitbox
    {
        Vector2 Position { get; set; }
    }

    public struct CircularHitbox : Hitbox
    {
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                diameter = value * 2;
            }
        }

        private float radius;

        public float Diameter
        {
            get => diameter;
            set
            {
                diameter = value;
                radius = value / 2;
            }
        }

        private float diameter;

        public Vector2 Position { get; set; }
    }

    public struct RectangularHitbox : Hitbox
    {
        public Vector2 Size;

        public float Rotation;

        public Vector2 Position { get; set; }
    }

    public struct HitResults
    {
        public float Distance;

        public float EdgeDistance;
    }

    public static class HitDetector
    {
        public static bool HitDetectionPossible(this CircularHitbox a, CircularHitbox b)
        {
            float min = a.Radius + b.Radius;

            if (a.Position.Y - b.Position.Y < min && a.Position.X - b.Position.X < min)
                return true;
            return false;
        }

        public static HitResults? HitDetectionResults(this CircularHitbox a, CircularHitbox b)
        {
            float distance = Vector2.Distance(b.Position, a.Position);
            return new HitResults
            {
                Distance = distance,
                EdgeDistance = distance - (a.Radius + b.Radius)
            };
        }

        public static bool HitDetectionPossible(this RectangularHitbox a, CircularHitbox b)
        {
            return false;
        }

        public static HitResults? HitDetectionResults(this RectangularHitbox a, CircularHitbox b)
        {
            float distance = Vector2.Distance(b.Position, a.Position);
            return new HitResults
            {
                Distance = distance,
                EdgeDistance = float.MaxValue
            };
        }
    }
}
