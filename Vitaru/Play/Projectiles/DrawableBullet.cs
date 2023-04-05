// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Graphics.Projectiles.Bullets;

namespace Vitaru.Play.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        public DrawableBullet(BulletLayer layer, int location) : base(layer, location)
        {
        }
    }
}
