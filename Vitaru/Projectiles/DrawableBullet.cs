// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;

namespace Vitaru.Projectiles
{
    public class DrawableBullet : DrawableProjectile<Bullet>
    {
        public DrawableBullet(Bullet bullet) : base(bullet)
        {
            Size = new Vector2(10);
            Color = Color.Magenta;
        }
    }
}