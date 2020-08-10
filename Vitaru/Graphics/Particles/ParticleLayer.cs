// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.IO;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.VAOs;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Nucleus;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<Particle>
    {
        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        private static ShaderProgram particleProgram;

        private VertexArrayObject<Vertex2> vao;

        public ParticleLayer()
        {
            if (Vitaru.FEATURES < Features.Experimental || particleProgram != null) return;

            Shader pv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader pf = Renderer.ShaderManager.GetShader(ShaderType.Pixel, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.frag")).ReadToEnd());

            particleProgram = Renderer.ShaderManager.GetShaderProgram(pv, pf);

            GLShaderProgram particle = (GLShaderProgram)particleProgram;

            particle.SetActive();

            particle.Locations["projection"] = GLShaderManager.GetLocation(particle, "projection");
            particle.Locations["model"] = GLShaderManager.GetLocation(particle, "model");
            particle.Locations["size"] = GLShaderManager.GetLocation(particle, "size");
            particle.Locations["spriteTexture"] = GLShaderManager.GetLocation(particle, "spriteTexture");
            particle.Locations["alpha"] = GLShaderManager.GetLocation(particle, "alpha");
            particle.Locations["spriteColor"] = GLShaderManager.GetLocation(particle, "spriteColor");
            particle.Locations["shade"] = GLShaderManager.GetLocation(particle, "shade");
            particle.Locations["intensity"] = GLShaderManager.GetLocation(particle, "intensity");

            Renderer.ShaderManager.ActiveShaderProgram = particle;

            Renderer.ShaderManager.UpdateInt("spriteTexture", 0);
            Renderer.ShaderManager.UpdateInt("shade", 0);
            Renderer.ShaderManager.UpdateInt("intensity", 1);

            Renderer.OnResize += value =>
            {
                particle.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = particle;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter(
                    Renderer.Width / -2f,
                    Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };
        }

        //Draw Particles Effeciently
        public override void Render()
        {
            if (!ProtectedChildren.Any()) return;

            if (Vitaru.FEATURES <= Features.Standard)
                software();
            else
                hardware();
        }

        private void hardware()
        {

        }

        private void software()
        {
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