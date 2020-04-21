// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Graphics.Transforms;

namespace Vitaru.Gamemodes.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        public DrawableBullet(Bullet bullet) : base(bullet)
        {
            Size = new Vector2(bullet.Diameter);
            Texture = Game.TextureStore.GetTexture("circle.png");
            Alpha = 0;
            Scale = new Vector2(1.5f);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            this.FadeTo(1, 150f, Easings.InSine);
            this.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }
    }
}