using System.Numerics;
using Vitaru.Settings;

namespace Vitaru.Play.Projectiles
{
    public class Laser : Projectile
    {
        public static int COUNT;

        private readonly bool particles = global::Vitaru.Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private readonly float particles_multiplier =
            global::Vitaru.Vitaru.VitaruSettings.GetFloat(VitaruSetting.ParticleMultiplier);

        public override string Name { get; set; } = nameof(Laser);

        public override DrawableGameEntity GenerateDrawable() => null;

        public override Hitbox GetHitbox() => RectangularHitbox;

        public RectangularHitbox RectangularHitbox = new()
        {
            Size = new Vector2(4, 10)
        };

        public Vector2 Size { get; set; }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            COUNT--;
        }
    }
}
