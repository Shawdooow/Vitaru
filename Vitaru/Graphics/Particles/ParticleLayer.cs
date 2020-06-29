// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Linq;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Vitaru.Settings;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<Particle>
    {
        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly bool gpu = Renderer._3D_AVAILABLE &&
                                    Vitaru.VitaruSettings.GetValue<bool>(VitaruSetting.GPUParticles);

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        //Draw Particles Effeciently
        public override void Render()
        {
            if (!ProtectedChildren.Any()) return;

            p.Start();

            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;

            //Shouldn't need to update them since they all should be the same. . .
            Renderer.ShaderManager.UpdateVector2("size", ProtectedChildren[0].Size);
            Renderer.CurrentContext.BindTexture(ProtectedChildren[0].Texture);

            //These will likely vary between particles, update them. . .
            for (int i = 0; i < ProtectedChildren.Count; i++)
            {
                Renderer.ShaderManager.UpdateMatrix4("model", ProtectedChildren[i].DrawTransform);
                Renderer.ShaderManager.UpdateFloat("alpha", ProtectedChildren[i].DrawAlpha);
                Renderer.ShaderManager.UpdateVector3("spriteColor", ProtectedChildren[i].DrawColor);
                Renderer.CurrentContext.RenderSpriteQuad();
            }

            p.Record();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Logger.Benchmark(p);
        }
    }
}