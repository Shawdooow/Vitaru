// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Settings;

namespace Vitaru.Play.Projectiles
{
    public class Laser : Projectile
    {
        public static int COUNT;

        private readonly bool particles = Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private readonly float particles_multiplier =
            Vitaru.VitaruSettings.GetFloat(VitaruSetting.ParticleMultiplier);

        public override string Name { get; set; } = nameof(Laser);

        public override DrawableGameEntity GenerateDrawable() => null;

        public override Hitbox GetHitbox() => RectangularHitbox;

        public RectangularHitbox RectangularHitbox = new()
        {
            Size = new Vector2(4, 10),
        };

        public Vector2 Size { get; set; }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            COUNT--;
        }
    }
}