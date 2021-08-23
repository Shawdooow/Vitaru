// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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
        public Vector2 Size { get; set; }

        public Vector2 Position { get; set; }
    }

    public struct HitResults
    {
        public Vector2 Position;

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

        public static HitResults HitDetectionResults(this CircularHitbox a, CircularHitbox b)
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

        public static bool HitDetectionResults(this RectangularHitbox a, RectangularHitbox b)
        {
            return a.Position.X < b.Position.X + b.Size.X &&
               a.Position.X + a.Size.X > b.Position.X &&
               a.Position.Y < b.Position.Y + b.Size.Y &&
               a.Position.Y + a.Size.Y > b.Position.Y;
        }
    }
}
