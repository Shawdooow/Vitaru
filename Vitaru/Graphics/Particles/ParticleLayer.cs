// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.VertexArray;
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
        private readonly int particle_cap = Vitaru.VitaruSettings.GetInt(VitaruSetting.ParticleCap);

        private const int vertLocation = 10;

        public static int PARTICLES_IN_USE { get; private set; }

        public override string Name { get; set; } = nameof(ParticleLayer);

        private readonly bool particles = Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private ShaderProgram program;

        private Texture texture;

        private int verts;

        public readonly float[] pLifetime;

        public readonly Vector4[] pPositions;

        public readonly Vector4[] pColor;

        private readonly VertexArrayBuffer<float> lifeBuffer;
        private readonly VertexArrayBuffer<Vector4> positionBuffer;
        private readonly VertexArrayBuffer<Vector4> colorBuffer;

        public readonly bool[] pDead;

        private readonly Stack<int> dead = new Stack<int>();

        private bool bufferParts;

        public ParticleLayer()
        {
            pLifetime = new float[particle_cap];
            pPositions = new Vector4[particle_cap];
            pColor = new Vector4[particle_cap];
            pDead = new bool[particle_cap];

            lifeBuffer = new VertexArrayBuffer<float>(ref pLifetime, 1, 11);
            positionBuffer = new VertexArrayBuffer<Vector4>(ref pPositions, 4, 12);
            colorBuffer = new VertexArrayBuffer<Vector4>(ref pColor, 4, 13);

            for (int i = particle_cap - 1; i >= 0; i--)
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

            Shader vert = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader frag = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                new StreamReader(Vitaru.ShaderStorage.GetStream("particle.frag")).ReadToEnd());

            program = Renderer.ShaderManager.GetShaderProgram(vert, frag);
            program.SetActive();

            GLShaderProgram gl = (GLShaderProgram) program;

            gl.Locations["projection"] = GLShaderManager.GetLocation(program, "projection");
            gl.Locations["size"] = GLShaderManager.GetLocation(program, "size");

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

            lifeBuffer.InitBuffer();
            positionBuffer.InitBuffer();
            colorBuffer.InitBuffer();

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

            if (particles)
                for (int i = 0; i < pLifetime.Length; i++)
                {
                    pLifetime[i] += last / 1200;

                    if (pLifetime[i] < 1)
                        PARTICLES_IN_USE++;
                    else if (!pDead[i])
                    {
                        pDead[i] = true;
                        dead.Push(i);
                    }
                }
        }

        public override void PreRender()
        {
            base.PreRender();

            if (!particles) return;

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            lifeBuffer.Buffer();

            if (bufferParts)
            {
                bufferParts = false;
                positionBuffer.Buffer();
                colorBuffer.Buffer();
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
            lifeBuffer.Bind();
            positionBuffer.Bind();
            colorBuffer.Bind();

            GL.VertexAttribDivisor(vertLocation, 0);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, particle_cap);

            GL.DisableVertexAttribArray(vertLocation);
            lifeBuffer.UnBind();
            positionBuffer.UnBind();
            colorBuffer.UnBind();

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public void Add(Particle particle)
        {
            if (!dead.Any()) return;

            int i = dead.Pop();

            Vector2 start = particle.StartPosition * Scale;
            Vector2 end = particle.EndPosition * Scale;

            pLifetime[i] = 0;
            pPositions[i] = new Vector4(start.X, start.Y, end.X, end.Y);
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

        protected override void Dispose(bool finalize)
        {
            GL.DeleteBuffer(verts);
            lifeBuffer.Dispose();
            positionBuffer.Dispose();
            colorBuffer.Dispose();
            program.Dispose();

            base.Dispose(finalize);
        }
    }
}