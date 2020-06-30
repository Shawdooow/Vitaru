// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.IO;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.SSBOs;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly ShaderProgram particleProgram;
        private readonly SSBO<Particle> particles;

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        public ParticleLayer()
        {
            Debugger.Assert(Renderer._3D_AVAILABLE);

            Shader comp = new GLShader(ShaderType.Compute, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.comp")).ReadToEnd());
            Shader vert = new GLShader(ShaderType.Vertex, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader frag = new GLShader(ShaderType.Pixel, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.frag")).ReadToEnd());

            particleProgram = new GLShaderProgram(comp, vert, frag);
            particles = new SSBO<Particle>(2);
        }

        //Draw Particles Effeciently
        public override void Render()
        {
            if (!ProtectedChildren.Any()) return;

            p.Start();

            //Render them!
            GL.DispatchCompute(1, 1, 1);

            p.Record();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Logger.Benchmark(p);
        }
    }
}