// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;

namespace Vitaru.Gamemodes.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        public override string Name { get; set; } = nameof(DrawableBullet);

        protected Sprite Glow;
        //protected Sprite OutlineCircle;
        protected Sprite CenterCircle;

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
                CenterCircle = new Circle()
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
            CenterCircle.Size = new Vector2(bullet.Diameter);

            return base.SetProjectile(projectile);
        }

        public override void Start()
        {
            base.Start();
            this.FadeTo(1, 200f, Easings.InSine);
            this.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Glow.Texture = Game.TextureStore.GetTexture("Gameplay\\glow.png");
            Start();
        }
    }
}