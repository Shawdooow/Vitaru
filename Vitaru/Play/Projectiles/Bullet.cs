// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Play.Projectiles
{
    public class Bullet : Projectile, IHasAlpha, IHasScale, IHasPosition
    {
        public override string Name { get; set; } = nameof(Bullet);

        public override Hitbox GetHitbox() => CircularHitbox;

        public CircularHitbox CircularHitbox = new()
        {
            Diameter = 10,
        };

        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                CircularHitbox.Position = value;
            }
        }

        public override Vector2 Size
        {
            get => base.Size;
            set => CircularHitbox.Diameter = value.X;
        }

        public override float Alpha
        {
            get => base.Alpha;
            set => base.Alpha = Math.Clamp(value, 0, 1);
        }
    }

    public enum Shape
    {
        Circle,
        Triangle,
        Square,
    }
}