// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Textures;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Nucleus.Debug;

namespace Vitaru.Graphics.Projectiles.Bullets
{
    public class BulletLayer : Layer2D<IDrawable2D>
    {
        public const int MAX_BULLETS = 4000;

        private const int vertLocation = 10;
        private const int positionLocation = 11;
        private const int sizeLocation = 12;
        private const int colorLocation = 13;

        public override string Name { get; set; } = nameof(BulletLayer);

        private int[] textures;

        private ShaderProgram program;

        private static int verts;
        private static int poss;
        private static int sizes;
        private static int colors;

        public readonly Vector2[] bPosition = new Vector2[MAX_BULLETS];

        public readonly Vector2[] bSize = new Vector2[MAX_BULLETS];

        public readonly Vector4[] bColor = new Vector4[MAX_BULLETS];

        private readonly IntPtr posBuffer;
        private readonly IntPtr sizeBuffer;
        private readonly IntPtr colorBuffer;

        public readonly bool[] bDead = new bool[MAX_BULLETS];

        private readonly Stack<int> dead = new Stack<int>();

        private int nCap;
        private int oCap;

        public BulletLayer()
        {
            GCHandle p = GCHandle.Alloc(bPosition, GCHandleType.Pinned);
            posBuffer = p.AddrOfPinnedObject();

            GCHandle s = GCHandle.Alloc(bSize, GCHandleType.Pinned);
            sizeBuffer = s.AddrOfPinnedObject();

            GCHandle c = GCHandle.Alloc(bColor, GCHandleType.Pinned);
            colorBuffer = c.AddrOfPinnedObject();

            for (int i = MAX_BULLETS - 1; i >= 0; i--)
            {
                bDead[i] = true;
                dead.Push(i);
            }
        }

        public override void LoadingComplete()
        {
            Debugger.Assert(Game.DrawThreaded);

            textures = new[]
            {
                ((GLTexture) Game.TextureStore.GetTexture("circle 128.png")).ID,
                ((GLTexture) Game.TextureStore.GetTexture("Gameplay\\glow.png")).ID,
            };

            program = Vitaru.BulletProgram;

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

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

            poss = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, poss);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);

            sizes = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, sizes);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);

            colors = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);

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

        public override void PreRender()
        {
            base.PreRender();

            oCap = Math.Min(MAX_BULLETS, nCap + 1);

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            GL.BindBuffer(BufferTarget.ArrayBuffer, poss);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 8, posBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, sizes);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 8, sizeBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, oCap * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, oCap * 16, colorBuffer);

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public override void Render()
        {
            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            // verts
            GL.EnableVertexAttribArray(vertLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.VertexAttribPointer(vertLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // positions
            GL.EnableVertexAttribArray(positionLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, poss);
            GL.VertexAttribPointer(positionLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // sizes
            GL.EnableVertexAttribArray(sizeLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, sizes);
            GL.VertexAttribPointer(sizeLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // colors
            GL.EnableVertexAttribArray(colorLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            GL.VertexAttribDivisor(vertLocation, 0);
            GL.VertexAttribDivisor(positionLocation, 2);
            GL.VertexAttribDivisor(sizeLocation, 2);
            GL.VertexAttribDivisor(colorLocation, 2);

            Renderer.ShaderManager.UpdateInt("circleTexture", 0);
            Renderer.ShaderManager.UpdateInt("glowTexture", 1);

            GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textures[1]);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, oCap * 2);

            GL.DisableVertexAttribArray(vertLocation);
            GL.DisableVertexAttribArray(positionLocation);
            GL.DisableVertexAttribArray(sizeLocation);
            GL.DisableVertexAttribArray(colorLocation);

            GL.ActiveTexture(TextureUnit.Texture0);
            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public int RequestIndex()
        {
            if (!dead.Any()) return -1;

            int i = dead.Pop();
            bDead[i] = false;
            nCap = Math.Max(i, nCap);
            return i;
        }

        public void ReturnIndex(int i)
        {
            dead.Push(i);
            bDead[i] = true;
            bColor[i].W = 0;
        }

        public override void Add(IDrawable2D child, AddPosition position = AddPosition.Last)
        {
            Debugger.InvalidOperation("Use Add(Bullet)");
        }

        public override void Remove(IDrawable2D child, bool dispose = true)
        {
            Debugger.InvalidOperation("Don't do this, they should be removed automatically");
        }
    }
}