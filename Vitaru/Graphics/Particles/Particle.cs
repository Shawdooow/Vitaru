using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Utilities;

namespace Vitaru.Graphics.Particles
{    /// <summary>
     ///     A <see cref="Sprite" /> with the "particle.png" <see cref="Texture" />
     /// </summary>
    public class Particle : Sprite
     {
        public readonly Vector2 End;

        public Particle(Vector2 start) : base(Game.TextureStore.GetTexture("particle.png"))
        {
            Position = start;
            Alpha = 0.8f;
            Scale = new Vector2(0.5f);

            float angle = ((float) PrionMath.RandomNumber(0, 360)).ToRadians();
            int distance = PrionMath.RandomNumber(10, 20);

            End = start + PrionMath.Offset(distance, angle);
        }
     }
}
