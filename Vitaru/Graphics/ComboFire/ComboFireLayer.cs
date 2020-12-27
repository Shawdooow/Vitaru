// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Vitaru.Graphics.Particles;
using Vitaru.Settings;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.ComboFire
{
    public class ComboFireLayer : Layer2D<IDrawable2D>
    {
        private readonly int flame_cap = 8192;

        private readonly bool combofire = Vitaru.VitaruSettings.GetBool(VitaruSetting.ComboFire);

        private ShaderProgram program;

        private Texture texture;

        private int verts;

        public readonly float[] fLifetime;

        public readonly Vector4[] fPositions;

        public readonly Vector4[] fColor;

        private readonly VertexArrayBuffer<float> lifeBuffer;
        private readonly VertexArrayBuffer<Vector4> positionBuffer;
        private readonly VertexArrayBuffer<Vector4> colorBuffer;

        public readonly bool[] fDead;

        private readonly ConcurrentStack<int> dead = new ConcurrentStack<int>();

        private bool bufferParts;

        public ComboFireLayer()
        {
            fLifetime = new float[flame_cap];
            fPositions = new Vector4[flame_cap];
            fColor = new Vector4[flame_cap];
            fDead = new bool[flame_cap];

            lifeBuffer = new VertexArrayBuffer<float>(ref fLifetime, 1, 11);
            positionBuffer = new VertexArrayBuffer<Vector4>(ref fPositions, 4, 12);
            colorBuffer = new VertexArrayBuffer<Vector4>(ref fColor, 4, 13);

            for (int i = flame_cap - 1; i >= 0; i--)
            {
                fLifetime[i] = 1;
                fDead[i] = true;
                dead.Push(i);
            }
        }

        public override void LoadingComplete()
        {
            Debugger.Assert(Game.DrawThreaded);

            texture = Game.TextureStore.GetTexture("flame.png");

            Shader vert = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                new StreamReader(Vitaru.ShaderStorage.GetStream("combo_fire.vert")).ReadToEnd());
            Shader frag = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                new StreamReader(Vitaru.ShaderStorage.GetStream("combo_fire.frag")).ReadToEnd());

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

        public void UpdateFlames(int start, int end, float last)
        {
            if (combofire)
            {
                float l = last / 1200;
                for (int i = start; i < end; i++)
                {
                    fLifetime[i] += l;

                    if (!fDead[i] && fLifetime[i] > 1)
                    {
                        fDead[i] = true;
                        dead.Push(i);
                    }
                }
            }
        }

        public override void PreRender()
        {
            base.PreRender();

            if (!combofire) return;

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
            if (!combofire) return;

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;
            Renderer.CurrentContext.BindTexture(texture);

            // verts
            GL.EnableVertexAttribArray(10);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.VertexAttribPointer(10, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // lifetime
            lifeBuffer.Bind();
            positionBuffer.Bind();
            colorBuffer.Bind();

            GL.VertexAttribDivisor(10, 0);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, flame_cap);

            GL.DisableVertexAttribArray(10);
            lifeBuffer.UnBind();
            positionBuffer.UnBind();
            colorBuffer.UnBind();

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public void Add(Particle flame)
        {
            if (!dead.TryPop(out int i)) return;

            Vector2 start = flame.StartPosition * Scale;
            Vector2 end = flame.EndPosition * Scale;

            fLifetime[i] = 0;
            fPositions[i] = new Vector4(start.X, start.Y, end.X, end.Y);
            fColor[i] = new Vector4(flame.Color, flame.Scale * Math.Min(Scale.X, Scale.Y));
            fDead[i] = false;
            bufferParts = true;
        }

        public override void Add(IDrawable2D child, AddPosition position = AddPosition.Last)
        {
            Debugger.InvalidOperation("Use Add(Flame)");
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