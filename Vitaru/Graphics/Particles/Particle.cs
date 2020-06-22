﻿using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Utilities;

namespace Vitaru.Graphics.Particles
{    /// <summary>
     ///     A <see cref="Sprite" /> with the "particle.png" <see cref="Texture" />
     /// </summary>
    public class Particle : Sprite
     {
        public static int COUNT;

        public readonly Vector2 End;

        public Particle(Vector2 start) : base(Game.TextureStore.GetTexture("particle.png"))
        {
            Position = start;
            Scale = new Vector2(1f / PrionMath.RandomNumber(1, 5));

            float angle = ((float) PrionMath.RandomNumber(0, 360)).ToRadians();
            int distance = PrionMath.RandomNumber(20, 40);

            End = start + PrionMath.Offset(distance, angle);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            COUNT++;
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            COUNT--;
        }
     }
}
