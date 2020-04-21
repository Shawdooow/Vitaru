// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;

namespace Vitaru.Gamemodes.Projectiles
{
    public class DrawableBullet : DrawableProjectile
    {
        protected Sprite Glow;
        protected Sprite OutlineCircle;
        protected Sprite CenterCircle;

        public DrawableBullet(Bullet bullet) : base(bullet)
        {
            Children = new[]
            {
                //Glow = new Sprite(Game.TextureStore.GetTexture("Gameplay\\Glow.png"))
                //{
                //    Size = new Vector2(bullet.Diameter * 2f),
                //    Color = bullet.Color,
                //},
                OutlineCircle = new Circle
                {
                    Alpha = 0,
                    Scale = new Vector2(1.5f),
                    Size = new Vector2(bullet.Diameter * 1.5f),
                    Color = bullet.Color,
                },
                CenterCircle = new Circle
                {
                    Alpha = 0,
                    Scale = new Vector2(1.5f),
                    Size = new Vector2(bullet.Diameter),
                },
            };
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            OutlineCircle.FadeTo(1, 200f, Easings.InSine);
            OutlineCircle.ScaleTo(Vector2.One, 100f, Easings.InSine);

            CenterCircle.FadeTo(1, 150f, Easings.InSine);
            CenterCircle.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }
    }
}