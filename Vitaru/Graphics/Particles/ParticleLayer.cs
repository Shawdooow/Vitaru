﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<IDrawable2D>
    {
        public const int MAX_PARTICLES = 20000;

        public static int PARTICLES_IN_USE { get; private set; }

        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        private static ShaderProgram program;

        private Texture texture;

        private static int verts;
        private static int life;
        private static int starts;
        private static int ends;
        private static int colors;

        public float[] pLifetime = new float[MAX_PARTICLES];

        public Vector2[] pStartPosition = new Vector2[MAX_PARTICLES];

        public Vector2[] pEndPosition = new Vector2[MAX_PARTICLES];

        public Vector4[] pColor = new Vector4[MAX_PARTICLES];

        private readonly IntPtr lifeBuffer = Marshal.AllocHGlobal(4 * MAX_PARTICLES);
        private readonly IntPtr startBuffer = Marshal.AllocHGlobal(8 * MAX_PARTICLES);
        private readonly IntPtr endBuffer = Marshal.AllocHGlobal(8 * MAX_PARTICLES);
        private readonly IntPtr colorBuffer = Marshal.AllocHGlobal(16 * MAX_PARTICLES);

        private bool bufferParts;

        public override void LoadingComplete()
        {
            Debugger.Assert(Game.DrawThreaded);

            texture = Game.TextureStore.GetTexture("particle.png");

            for (int i = 0; i < pLifetime.Length; i++)
                pLifetime[i] = 2;

            if (program != null) return;

            Shader vert = Renderer.ShaderManager.GetShader(ShaderType.Vertex, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader frag = Renderer.ShaderManager.GetShader(ShaderType.Pixel, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.frag")).ReadToEnd());

            program = Renderer.ShaderManager.GetShaderProgram(vert, frag);
            program.SetActive();

            GLShaderProgram gl = (GLShaderProgram)program;

            gl.Locations["projection"] = GLShaderManager.GetLocation(program, "projection");
            gl.Locations["spriteTexture"] = GLShaderManager.GetLocation(program, "spriteTexture");

            Renderer.ShaderManager.ActiveShaderProgram = program;
            Renderer.CurrentContext.BindTexture(texture);

            Vertex2[] array =
            {
                new Vertex2(new Vector2(-1f)),
                new Vertex2(new Vector2(-1f, 1f)),
                new Vertex2(new Vector2(1f, -1f)),
                new Vertex2(new Vector2(1f))
            };

            verts = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.BufferData(BufferTarget.ArrayBuffer, 8 * 4, array, BufferUsageHint.StaticDraw);

            life = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, life);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);

            starts = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, starts);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);

            ends = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ends);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);

            colors = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);

            Renderer.OnResize += value =>
            {
                program.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = program;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter( Renderer.Width / -2f, Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));
        }

        public void UpdateParticles()
        {
            float last = (float)Clock.LastElapsedTime;
            PARTICLES_IN_USE = 0;

            for (int i = 0; i < pLifetime.Length; i++)
            {
                pLifetime[i] += last / 1000;

                if (pLifetime[i] < 1) PARTICLES_IN_USE++;
            }
        }

        public override void PreRender()
        {
            base.PreRender();

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            bufferLife();

            if (bufferParts)
            {
                bufferParts = false;
                buffer();
            }

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        //Draw Particles Effeciently
        public override void Render()
        {
            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;
            Renderer.CurrentContext.BindTexture(texture);

            // verts
            GL.EnableVertexAttribArray(10);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.VertexAttribPointer(10, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // lifetime
            GL.EnableVertexAttribArray(11);
            GL.BindBuffer(BufferTarget.ArrayBuffer, life);
            GL.VertexAttribPointer(11, 1, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // start positions
            GL.EnableVertexAttribArray(12);
            GL.BindBuffer(BufferTarget.ArrayBuffer, starts);
            GL.VertexAttribPointer(12, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // end positions
            GL.EnableVertexAttribArray(13);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ends);
            GL.VertexAttribPointer(13, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // colors
            GL.EnableVertexAttribArray(14);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.VertexAttribPointer(14, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero); 

            GL.VertexAttribDivisor(10, 0);
            GL.VertexAttribDivisor(11, 1);
            GL.VertexAttribDivisor(12, 1);
            GL.VertexAttribDivisor(13, 1);
            GL.VertexAttribDivisor(14, 1);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, MAX_PARTICLES);

            GL.DisableVertexAttribArray(10);
            GL.DisableVertexAttribArray(11);
            GL.DisableVertexAttribArray(12);
            GL.DisableVertexAttribArray(13);
            GL.DisableVertexAttribArray(14);

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public void Add(Particle particle)
        {
            for (int i = 0; i < pLifetime.Length; i++)
                if (pLifetime[i] >= 1)
                {
                    pLifetime[i] = 0;
                    pStartPosition[i] = particle.StartPosition;
                    pEndPosition[i] = particle.EndPosition;
                    pColor[i] = new Vector4(particle.Color, particle.Scale);
                    bufferParts = true;
                    return;
                }

            Logger.Warning("Too many Particles!");
        }

        public override void Add(IDrawable2D child, AddPosition position = AddPosition.Last)
        {
            Debugger.InvalidOperation("Use Add(Particle)");
        }

        public override void Remove(IDrawable2D child, bool dispose = true)
        {
            Debugger.InvalidOperation("Don't do this, they should be removed automatically");
        }

        private void bufferLife()
        {
            byte[] l = Unsafe.As<float[], byte[]>(ref pLifetime);

            Marshal.Copy(l, 0, lifeBuffer, MAX_PARTICLES);

            GL.BindBuffer(BufferTarget.ArrayBuffer, life);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_PARTICLES * 4, lifeBuffer);
        }

        private void buffer()
        {
            byte[] s = Unsafe.As<Vector2[], byte[]>(ref pStartPosition);
            byte[] e = Unsafe.As<Vector2[], byte[]>(ref pEndPosition);
            byte[] c = Unsafe.As<Vector4[], byte[]>(ref pColor);

            Marshal.Copy(s, 0, startBuffer, MAX_PARTICLES);
            Marshal.Copy(e, 0, endBuffer, MAX_PARTICLES);
            Marshal.Copy(c, 0, colorBuffer, MAX_PARTICLES);

            GL.BindBuffer(BufferTarget.ArrayBuffer, starts);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_PARTICLES * 8, startBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ends);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_PARTICLES * 8, endBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_PARTICLES * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_PARTICLES * 16, colorBuffer);
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Logger.Benchmark(p);
        }
    }
}