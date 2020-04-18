// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Projectiles
{
    public abstract class DrawableProjectile : Sprite
    {
        protected DrawableProjectile(Projectile projectile)
        {
            Position = projectile.StartPosition;
        }

        public event Action OnDelete;

        /// <summary>
        ///     Tells this <see cref="DrawableProjectile" /> to remove itself from our Parent and Dispose
        /// </summary>
        public virtual void Delete() => OnDelete?.Invoke();
    }
}