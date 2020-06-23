// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Gamemodes.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        public override string Name { get; set; } = nameof(DrawableBullet);

        public readonly Sprite Glow;

        //protected Sprite OutlineCircle;
        public readonly Sprite Center;

        public DrawableBullet()
        {
            Children = new[]
            {
                Glow = new Sprite(),
                //OutlineCircle = new Circle
                //{
                //    Size = new Vector2(bullet.Diameter * 1.5f),
                //    Color = bullet.Color
                //},
                Center = new Circle()
            };
        }

        public override DrawableGameEntity SetProjectile(Projectile projectile)
        {
            //TODO: gross
            Bullet bullet = projectile as Bullet;

            Alpha = 0;
            Scale = new Vector2(1.5f);

            Glow.Size = new Vector2(bullet.Diameter * 3f);
            Glow.Color = bullet.Color;
            Center.Size = new Vector2(bullet.Diameter);

            return base.SetProjectile(projectile);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Glow.Texture = Game.TextureStore.GetTexture("Gameplay\\glow.png");
        }
    }
}