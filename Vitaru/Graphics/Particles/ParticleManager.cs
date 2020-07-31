using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria.Graphics.Contexts.GL46.SSBOs;
using Prion.Nucleus.Debug;

namespace Vitaru.Graphics.Particles
{
    public static class ParticleManager
    {
        internal static List<Matrix4x4> Master = new List<Matrix4x4>();

        private static List<ParticlePointer> pointers = new List<ParticlePointer>();

        internal static SSBO<Matrix4x4> Particles;

        public static void SetSSBO(SSBO<Matrix4x4> ssbo)
        {
            Reset();
            Particles?.Dispose();
            Particles = ssbo;
        }

        public static void UpdateParticles()
        {
            for (int i = 0; i < pointers.Count; i++)
                pointers[i].Update();
        }

        public static void UpdateSSBO() => Particles.Values = Master.ToArray();

        public static void Reset()
        {
            Master = new List<Matrix4x4>();
            pointers = new List<ParticlePointer>();
        }

        public static ParticlePointer GetParticle()
        {
            Master.Add(new Matrix4x4());

            pointers.Add(new ParticlePointer(Master.Last())
            {
                Index = Master.Count - 1
            });

            return pointers.Last();
        }

        public static void ReturnParticle(ParticlePointer pointer)
        {
            Logger.Warning("Returning Particles is WIP!", LogType.Graphics);

            Master.Remove(Master[pointer.Index]);
            pointers.Remove(pointer);

            for (int i = pointer.Index; i < pointers.Count; i++)
                pointers[i].Index--;
        }
    }
}
