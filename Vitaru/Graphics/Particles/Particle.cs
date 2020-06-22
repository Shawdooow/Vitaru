using System;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Nucleus.Utilities;

namespace Vitaru.Graphics.Particles
{    /// <summary>
     ///     A <see cref="Sprite" /> with the "particle.png" <see cref="Texture" />
     /// </summary>
    public class Particle : Sprite
     {
        private readonly Vector2 end;

        public event Action OnDelete;

        public Particle(Vector2 start) : base(Game.TextureStore.GetTexture("particle.png"))
        {
            Position = start;
            Alpha = 0.8f;
            Scale = new Vector2(0.5f);

            float angle = ((float) PrionMath.RandomNumber(0, 360)).ToRadians();
            int distance = PrionMath.RandomNumber(10, 20);

            end = start + PrionMath.Offset(distance, angle);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            this.MoveTo(end, 2000f, Easings.OutQuint).OnComplete(() => 
                this.FadeTo(0, 200, Easings.OutQuint).OnComplete(() => OnDelete?.Invoke()));
        }
     }
}
