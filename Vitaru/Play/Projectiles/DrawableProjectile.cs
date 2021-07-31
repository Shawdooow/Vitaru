// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Play.Projectiles
{
    public abstract class DrawableProjectile : DrawableGameEntity
    {
        public override string Name { get; set; } = nameof(DrawableProjectile);

        public virtual DrawableGameEntity SetProjectile(Projectile projectile)
        {
            Position = projectile.Position;
            return this;
        }
    }
}