// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.VAOs;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Nucleus;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<Particle>
    {
        public const int MAX_PARTICLES = 20000;

        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        private static ShaderProgram particleProgram;

        private readonly bool upcoming = Vitaru.FEATURES >= Features.Upcoming;

        private VertexArrayObject<Vertex2> vao;

        public ParticleLayer()
        {
            if (!upcoming || particleProgram != null) return;

            Vertex2[] array = new[]
            {
                new Vertex2(new Vector2(-1f)),
                new Vertex2(new Vector2(-1f, 1f)),
                new Vertex2(new Vector2(1f, -1f)),
                new Vertex2(new Vector2(1f))
            };

            int vert = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vert);
            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(array), array, BufferUsageHint.StaticDraw);

            // The VBO containing the positions and sizes of the particles
            int positions = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positions);
            // Initialize with empty (NULL) buffer : it will be updated later, each frame.
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 4 * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);

            // The VBO containing the colors of the particles
            int colors = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            // Initialize with empty (NULL) buffer : it will be updated later, each frame.
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);
        }

        //Draw Particles Effeciently
        public override void Render()
        {
            if (!ProtectedChildren.Any()) return;

            if (!upcoming)
                software();
            else
                hardware();
        }

        private void hardware()
        {
            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, ProtectedChildren.Count);
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