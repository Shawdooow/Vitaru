﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game;

namespace Vitaru.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        public DrawableBullet(Bullet bullet) : base(bullet)
        {
            Size = new Vector2(bullet.Diameter);
            Texture = Game.TextureStore.GetTexture("circle.png");
        }
    }
}