// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Utilities;

namespace Vitaru.Projectiles
{
    public class Bullet : Projectile
    {
        protected DrawableBullet Drawable { get; private set; }

        public DrawableBullet GenerateDrawableBullet() => Drawable = new DrawableBullet(this);

        public Vector2 EndPosition { get; protected set; }

        public float Distance { get; set; }

        public CurveType CurveType { get; set; }

        public int Curviness { get; set; }

        public float Speed { get; set; }

        public Easings SpeedEasing { get; set; }

        public float Diameter { get; set; }

        public Shape Shape { get; set; }

        public override void Update()
        {
        }
    }

    public enum CurveType
    {
        Straight,
        Target,

        CurveRight,
        CurveLeft,
    }

    public enum Shape
    {
        Circle,
        Triangle,
        Square
    }
}