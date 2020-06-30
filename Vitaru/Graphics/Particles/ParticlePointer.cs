namespace Vitaru.Graphics.Particles
{
    public class ParticlePointer
    {
        public int Index { get; internal set; }

        private Particle particle;

        internal ParticlePointer(Particle particle)
        {
            this.particle = particle;
        }

        public void Update() => particles.Master[Index] = particle;
    }
}
