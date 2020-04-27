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
        public override string Name { get; set; } = nameof(DrawableBullet);

        protected Sprite Glow;
        protected Sprite OutlineCircle;
        protected Sprite CenterCircle;

        public DrawableBullet(Bullet bullet) : base(bullet)
        {
            Alpha = 0;
            Scale = new Vector2(1.5f);

            Children = new[]
            {
                //Glow = new Sprite(Game.TextureStore.GetTexture("Gameplay\\Glow.png"))
                //{
                //    Size = new Vector2(bullet.Diameter * 2f),
                //    Color = bullet.Color,
                //},
                OutlineCircle = new Circle
                {
                    Size = new Vector2(bullet.Diameter * 1.5f),
                    Color = bullet.Color
                },
                CenterCircle = new Circle
                {
                    Size = new Vector2(bullet.Diameter)
                }
            };
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            this.FadeTo(1, 200f, Easings.InSine);
            this.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }
    }
}