// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Vitaru.Settings;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<IDrawable2D>
    {
        public const int MAX_PARTICLES = 64000;

        private const int vertLocation = 10;
        private const int lifetimeLocation = 11;
        private const int startLocation = 12;
        private const int endLocation = 13;
        private const int colorLocation = 14;

        public static int PARTICLES_IN_USE { get; private set; }

        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly bool particles = Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private static ShaderProgram program;

        private Texture texture;

        private static int verts;
        private static int life;
        private static int starts;
        private static int ends;
        private static int colors;

        public readonly float[] pLifetime = new float[MAX_PARTICLES];

        public readonly Vector2[] pStartPosition = new Vector2[MAX_PARTICLES];

        public readonly Vector2[] pEndPosition = new Vector2[MAX_PARTICLES];

        public readonly Vector4[] pColor = new Vector4[MAX_PARTICLES];

        private readonly IntPtr lifeBuffer;
        private readonly IntPtr startBuffer;
        private readonly IntPtr endBuffer;
        private readonly IntPtr colorBuffer;

        public readonly bool[] pDead = new bool[MAX_PARTICLES];

        private readonly Stack<int> dead = new Stack<int>();

        private int nCap;
        private int oCap;

        private bool bufferParts;

        public ParticleLayer()
        {
            GCHandle l = GCHandle.Alloc(pLifetime, GCHandleType.Pinned);
            lifeBuffer = l.AddrOfPinnedObject();

            GCHandle s = GCHandle.Alloc(pStartPosition, GCHandleType.Pinned);
            startBuffer = s.AddrOfPinnedObject();

            GCHandle e = GCHandle.Alloc(pEndPosition, GCHandleType.Pinned);
            endBuffer = e.AddrOfPinnedObject();

            GCHandle c = GCHandle.Alloc(pColor, GCHandleType.Pinned);
            colorBuffer = c.AddrOfPinnedObject();

            for (int i = MAX_PARTICLES - 1; i >= 0; i--)
            {
                pLifetime[i] = 1;
                pDead[i] = true;
                dead.Push(i);
            }
        }

        public override void LoadingComplete()
        {
            Debugger.Assert(Game.DrawThreaded);

            texture = Game.TextureStore.GetTexture("star.png");

            if (program != null) return;

            Shader vert = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader frag = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                new StreamReader(Vitaru.ShaderStorage.GetStream("particle.frag")).ReadToEnd());

            program = Renderer.ShaderManager.GetShaderProgram(vert, frag);
            program.SetActive();

            GLShaderProgram gl = (GLShaderProgram) program;

            gl.Locations["projection"] = GLShaderManager.GetLocation(program, "projection");
            gl.Locations["size"] = GLShaderManager.GetLocation(program, "size");
            //gl.Locations["spriteTexture"] = GLShaderManager.GetLocation(program, "spriteTexture");

            Renderer.ShaderManager.ActiveShaderProgram = program;
            Renderer.CurrentContext.BindTexture(texture);

            Renderer.ShaderManager.UpdateFloat("size", 16f);

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
                Renderer.ShaderManager.UpdateMatrix4("projection",
                    Matrix4x4.CreateOrthographicOffCenter(Renderer.Width / -2f, Renderer.Width / 2f,
                        Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));
        }

        public void UpdateParticles(float last)
        {
            PARTICLES_IN_USE = 0;
            int c = 0;

            if (particles)
                for (int i = 0; i < pLifetime.Length; i++)
                {
                    pLifetime[i] += last / 1200;

                    if (pLifetime[i] < 1)
                    {
                        PARTICLES_IN_USE++;
                        c = Math.Max(i, c);
                    }
                    else if (!pDead[i])
                    {
                        pDead[i] = true;
                        dead.Push(i);
                    }
                }

            nCap = Math.Min(MAX_PARTICLES, c + 1);
        }

        public override void PreRender()
        {
            base.PreRender();

            if (!particles) return;

            oCap = nCap;

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

        public override void Render()
        {
            if (!particles) return;

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;
            Renderer.CurrentContext.BindTexture(texture);

            // verts
            GL.EnableVertexAttribArray(vertLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.VertexAttribPointer(vertLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // lifetime
            GL.EnableVertexAttribArray(lifetimeLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, life);
            GL.VertexAttribPointer(lifetimeLocation, 1, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // start positions
            GL.EnableVertexAttribArray(startLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, starts);
            GL.VertexAttribPointer(startLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // end positions
            GL.EnableVertexAttribArray(endLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ends);
            GL.VertexAttribPointer(endLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // colors
            GL.EnableVertexAttribArray(colorLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            GL.VertexAttribDivisor(vertLocation, 0);
            GL.VertexAttribDivisor(lifetimeLocation, 1);
            GL.VertexAttribDivisor(startLocation, 1);
            GL.VertexAttribDivisor(endLocation, 1);
            GL.VertexAttribDivisor(colorLocation, 1);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, oCap);

            GL.DisableVertexAttribArray(vertLocation);
            GL.DisableVertexAttribArray(lifetimeLocation);
            GL.DisableVertexAttribArray(startLocation);
            GL.DisableVertexAttribArray(endLocation);
            GL.DisableVertexAttribArray(colorLocation);

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public void Add(Particle particle)
        {
            if (!dead.Any()) return;

            int i = dead.Pop();

            pLifetime[i] = 0;
            pStartPosition[i] = particle.StartPosition * Scale;
            pEndPosition[i] = particle.EndPosition * Scale;
            pColor[i] = new Vector4(particle.Color, particle.Scale * Math.Min(Scale.X, Scale.Y));
            pDead[i] = false;
            bufferParts = true;
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, life);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 4, lifeBuffer);
        }

        private void buffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, starts);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 8, startBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ends);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 8, endBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 16, colorBuffer);
        }
    }
}