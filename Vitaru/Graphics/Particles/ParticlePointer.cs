using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Graphics.Particles
{
    public class ParticlePointer
    {
        public int Index { get; internal set; }

        public Vector2 StartPosition
        {
            get => particle.StartPosition.XYUnsafeReadonly();
            set
            {
                particle.StartPosition.X = value.X;
                particle.StartPosition.Y = value.Y;
            }
        }

        public Vector2 EndPosition
        {
            get => particle.EndPosition.XYUnsafeReadonly();
            set
            {
                particle.EndPosition.X = value.X;
                particle.EndPosition.Y = value.Y;
            }
        }

        public Vector3 Color
        {
            get => particle.Color.XYZUnsafeReadonly();
            set
            {
                particle.Color.X = value.X;
                particle.Color.Y = value.Y;
                particle.Color.Z = value.Z;
            }
        }

        public float StartTime
        {
            get => particle.StartPosition.Z;
            set => particle.StartPosition.Z = value;
        }

        public float EndTime
        {
            get => particle.EndPosition.Z;
            set => particle.EndPosition.Z = value;
        }

        public float Scale
        {
            get => particle.StartPosition.W;
            set => particle.StartPosition.W = value;
        }

        public float Rotation
        {
            get => particle.EndPosition.W;
            set => particle.EndPosition.W = value;
        }

        public float Alpha
        {
            get => particle.Color.W;
            set => particle.Color.W = value;
        }

        private Particle particle;

        internal ParticlePointer(Particle particle)
        {
            this.particle = particle;
        }

        public void Update() => ParticleManager.Master[Index] = particle;
    }
}
