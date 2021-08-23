// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.Textures;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Verticies;
using Prion.Nucleus.Debug;
using Vitaru.Settings;

namespace Vitaru.Graphics.Projectiles.Bullets
{
    public class BulletLayer : Layer2D<IDrawable2D>
    {
        private readonly int bullet_cap = Vitaru.VitaruSettings.GetInt(VitaruSetting.BulletCap);

        private const int vertLocation = 10;

        public override string Name { get; set; } = nameof(BulletLayer);

        private TextureHandle[] textures;

        private ShaderProgram program;

        private static BufferHandle verts;

        public readonly Vector2[] bPosition;
        public readonly Vector2[] bSize;
        public readonly Vector4[] bColor;

        private readonly VertexArrayBuffer<Vector2> posBuffer;
        private readonly VertexArrayBuffer<Vector2> sizeBuffer;
        private readonly VertexArrayBuffer<Vector4> colorBuffer;

        public readonly bool[] bDead;

        private readonly ConcurrentStack<int> dead = new();

        public BulletLayer()
        {
            bPosition = new Vector2[bullet_cap];
            bSize = new Vector2[bullet_cap];
            bColor = new Vector4[bullet_cap];
            bDead = new bool[bullet_cap];

            posBuffer = new VertexArrayBuffer<Vector2>(ref bPosition, 2, 11);
            sizeBuffer = new VertexArrayBuffer<Vector2>(ref bSize, 2, 12);
            colorBuffer = new VertexArrayBuffer<Vector4>(ref bColor, 4, 13);

            for (int i = bullet_cap - 1; i >= 0; i--)
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
                ((GLTexture) Game.TextureStore.GetTexture("circle 128.png")).Handle,
                ((GLTexture) Game.TextureStore.GetTexture("Gameplay\\glow.png")).Handle
            };

            program = Vitaru.BulletProgram;

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            Vertex2[] array =
            {
                new(new Vector2(-1f)),
                new(new Vector2(-1f, 1f)),
                new(new Vector2(1f, -1f)),
                new(new Vector2(1f))
            };

            GCHandle h = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr address = h.AddrOfPinnedObject();

            verts = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, verts);
            GL.BufferData(BufferTargetARB.ArrayBuffer, 8 * 4, address, BufferUsageARB.StaticDraw);

            posBuffer.InitBuffer();
            sizeBuffer.InitBuffer();
            colorBuffer.InitBuffer();

            Renderer.OnResize += value =>
            {
                program.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = program;
                Renderer.ShaderManager.UpdateMatrix4("projection",
                    Matrix4x4.CreateOrthographicOffCenter(Renderer.Size.X / -2f, Renderer.Size.X / 2f,
                        Renderer.Size.Y / 2f, Renderer.Size.Y / -2f, 1, -1));
            };

            Renderer.OnResize.Invoke(Renderer.RenderSize);
        }

        public override void PreRender()
        {
            base.PreRender();

            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            Renderer.ShaderManager.UpdateVector2("scale", Scale);

            posBuffer.Buffer();
            sizeBuffer.Buffer();
            colorBuffer.Buffer();

            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public override void Render()
        {
            program.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = program;

            // verts
            GL.EnableVertexAttribArray(vertLocation);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, verts);
            GL.VertexAttribPointer(vertLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            posBuffer.Bind(2);
            sizeBuffer.Bind(2);
            colorBuffer.Bind(2);

            GL.VertexAttribDivisor(vertLocation, 0);

            Renderer.ShaderManager.UpdateInt("circleTexture", 0);
            Renderer.ShaderManager.UpdateInt("glowTexture", 1);

            GL.BindTexture(TextureTarget.Texture2d, textures[0]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2d, textures[1]);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, bullet_cap * 2);

            GL.DisableVertexAttribArray(vertLocation);
            posBuffer.UnBind();
            sizeBuffer.UnBind();
            colorBuffer.UnBind();

            GL.ActiveTexture(TextureUnit.Texture0);
            Renderer.SpriteProgram.SetActive();
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
        }

        public int RequestIndex()
        {
            if (!dead.TryPop(out int i)) return -1;

            bDead[i] = false;
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
            Debugger.InvalidOperation("Use int RequestIndex()");
        }

        public override void Remove(IDrawable2D child, bool dispose = true)
        {
            Debugger.InvalidOperation("Use ReturnIndex(int i)");
        }

        protected override void Dispose(bool finalize)
        {
            GL.DeleteBuffer(verts);
            posBuffer.Dispose();
            sizeBuffer.Dispose();
            colorBuffer.Dispose();
            base.Dispose(finalize);
        }
    }
}