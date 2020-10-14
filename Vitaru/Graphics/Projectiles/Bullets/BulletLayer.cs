using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Vitaru.Gamemodes.Projectiles;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Projectiles.Bullets
{
    public class BulletLayer : ShadeLayer<IDrawable2D>
    {
        public const int MAX_BULLETS = 4000;

        public override string Name { get; set; } = nameof(BulletLayer);

        private Texture[] textures;

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

        public BulletLayer()
        {
            GCHandle p = GCHandle.Alloc(bPosition, GCHandleType.Pinned);
            posBuffer = p.AddrOfPinnedObject();

            GCHandle s = GCHandle.Alloc(bSize, GCHandleType.Pinned);
            sizeBuffer = s.AddrOfPinnedObject();

            GCHandle c = GCHandle.Alloc(bColor, GCHandleType.Pinned);
            colorBuffer = c.AddrOfPinnedObject();

            for (int i = 0; i < MAX_BULLETS; i++)
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
                Game.TextureStore.GetTexture("Gameplay\\glow.png"),
                Game.TextureStore.GetTexture("circle 128.png")
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

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            GL.BindBuffer(BufferTarget.ArrayBuffer, poss);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_BULLETS * 8, posBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, sizes);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 8, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_BULLETS * 8, sizeBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_BULLETS * 16, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MAX_BULLETS * 16, colorBuffer);

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public override void Render()
        {
            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            //Renderer.ShaderManager.UpdateInt("shade", (int)Shade);
            //Renderer.ShaderManager.UpdateFloat("intensity", Intensity);

            // verts
            GL.EnableVertexAttribArray(20);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verts);
            GL.VertexAttribPointer(20, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // positions
            GL.EnableVertexAttribArray(21);
            GL.BindBuffer(BufferTarget.ArrayBuffer, poss);
            GL.VertexAttribPointer(21, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // sizes
            GL.EnableVertexAttribArray(22);
            GL.BindBuffer(BufferTarget.ArrayBuffer, sizes);
            GL.VertexAttribPointer(22, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            // colors
            GL.EnableVertexAttribArray(23);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
            GL.VertexAttribPointer(23, 4, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            GL.VertexAttribDivisor(20, 0);
            GL.VertexAttribDivisor(21, 1);
            GL.VertexAttribDivisor(22, 1);
            GL.VertexAttribDivisor(23, 1);

            //Renderer.ShaderManager.UpdateInt("white", 0);
            //Renderer.CurrentContext.BindTexture(textures[0]);
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, MAX_BULLETS);

            //Renderer.ShaderManager.UpdateInt("white", 1);
            //Renderer.CurrentContext.BindTexture(textures[1]);
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, MAX_BULLETS);

            GL.DisableVertexAttribArray(20);
            GL.DisableVertexAttribArray(21);
            GL.DisableVertexAttribArray(22);
            GL.DisableVertexAttribArray(23);

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public int RequestIndex()
        {
            int i = dead.Pop();
            bDead[i] = false;
            return i;
        }

        public void ReturnIndex(int i)
        {
            dead.Push(i);
            bDead[i] = true;
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
