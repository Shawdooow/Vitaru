using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria.Graphics.Contexts.GL46.SSBOs;
using Prion.Nucleus.Debug;

namespace Vitaru.Graphics.Particles
{
    public static class ParticleManager
    {
        internal static ConcurrentQueue<Matrix4x4> Master = new ConcurrentQueue<Matrix4x4>();

        private static ConcurrentQueue<ParticlePointer> pointers = new ConcurrentQueue<ParticlePointer>();

        internal static SSBO<Matrix4x4> Particles;

        public static void SetSSBO(SSBO<Matrix4x4> ssbo)
        {
            Reset();
            Particles?.Dispose();
            Particles = ssbo;
        }

        public static void UpdateParticles()
        {
            foreach (ParticlePointer p in pointers)
                p.Update();
        }

        public static void UpdateSSBO() => Particles.Values = Master.ToArray();

        public static void Reset()
        {
            Master = new ConcurrentQueue<Matrix4x4>();
            pointers = new ConcurrentQueue<ParticlePointer>();
        }

        public static ParticlePointer GetParticle()
        {
            Master.Enqueue(new Matrix4x4());

            pointers.Enqueue(new ParticlePointer(Master.Last())
            {
                Index = Master.Count - 1
            });

            return pointers.Last();
        }

        public static void RemoveOldParticles(double time)
        {
            while (pointers.Any() && pointers.First().EndTime <= time)
                remove();
        }

        private static void remove()
        {
            Debugger.Assert(Master.TryDequeue(out Matrix4x4 mat4));
            Debugger.Assert(pointers.TryDequeue(out ParticlePointer pointer));

            ParticlePointer[] e = pointers.ToArray();
            for (int i = pointer.Index; i < e.Length; i++)
                e[i].Index--;
        }
    }
}
