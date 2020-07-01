// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.SSBOs;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

namespace Vitaru.Graphics.Particles
{
    public class ParticleLayer : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(ParticleLayer);

        private GLShaderProgram compute;
        private GLShaderProgram render;

        private readonly Benchmark p = new Benchmark("Particle Render Time");

        private Texture texture;

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Debugger.Assert(Renderer._3D_AVAILABLE);

            texture = Game.TextureStore.GetTexture("circle.png");

            Shader comp = new GLShader(ShaderType.Compute, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.comp")).ReadToEnd());
            Shader vert = new GLShader(ShaderType.Vertex, new StreamReader(Vitaru.ShaderStorage.GetStream("particle.vert")).ReadToEnd());
            Shader frag = new GLShader(ShaderType.Pixel, new StreamReader(Vitaru.ShaderStorage.GetStream("sprite_shade.frag")).ReadToEnd());

            compute = new GLShaderProgram(comp);
            compute.SetActive();

            compute.Locations["time"] = GLShaderManager.GetLocation(compute, "time");
            compute.Locations["parentTransform"] = GLShaderManager.GetLocation(compute, "parentTransform");

            render = new GLShaderProgram(vert, frag);
            render.SetActive();

            render.Locations["projection"] = GLShaderManager.GetLocation(render, "projection");
            render.Locations["index"] = GLShaderManager.GetLocation(render, "index");
            render.Locations["size"] = GLShaderManager.GetLocation(render, "size");
            render.Locations["spriteTexture"] = GLShaderManager.GetLocation(render, "spriteTexture");
            render.Locations["shade"] = GLShaderManager.GetLocation(render, "shade");
            render.Locations["intensity"] = GLShaderManager.GetLocation(render, "intensity");

            Renderer.ShaderManager.ActiveShaderProgram = render;

            Renderer.ShaderManager.UpdateInt("spriteTexture", 0);
            Renderer.ShaderManager.UpdateInt("shade", 0);
            Renderer.ShaderManager.UpdateInt("intensity", 1);

            Renderer.OnResize += value =>
            {
                render.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = render;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter(
                    Renderer.Width / -2f,
                    Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));

            ParticleManager.SetSSBO(new SSBO<Particle>(2));

            ParticlePointer particle = ParticleManager.GetParticle();
            
            particle.StartPosition = new Vector2(-100, 200);
            particle.EndPosition = new Vector2(200);
            particle.Color = new Vector3(0, 0, 1);
            particle.StartTime = 0;
            particle.EndTime = 2000;
            particle.Scale = 5f;
            particle.Rotation = 0f;
            particle.Alpha = 1f;
            particle.Update();

            Matrix4x4 scale = Matrix4x4.CreateScale(new Vector3(particle.Scale));
            Matrix4x4 rotation = Matrix4x4.CreateRotationZ(particle.Rotation, Vector3.Zero);
            Matrix4x4 translate = Matrix4x4.CreateTranslation(get(particle.StartPosition));
            
            Matrix4x4 draw = Matrix4x4.Identity;
            
            draw *= scale;
            draw *= rotation;
            draw *= translate;
            draw *= TotalTransform;
            
            Logger.SystemConsole(draw.ToString(), ConsoleColor.Blue);
            
            Vector3 get(Vector2 position)
            {
                Vector2 drawOffset = position * particle.Scale;
            
                Vector3 totalOffset = new Vector3(drawOffset, 0);
            
                return totalOffset;
            }
        }

        private bool sdasds;

        //Draw Particles Effeciently
        public override void Render()
        {
            if (!ParticleManager.Master.Any()) return;

            if (!sdasds)
            {
                Logger.SystemConsole(TotalTransform.ToString(), ConsoleColor.Blue);
                sdasds = true;
            }

            p.Start();

            Renderer.ShaderManager.ActiveShaderProgram = compute;
            compute.SetActive();

            //Update Particle info
            ParticleManager.UpdateSSBO();
            Renderer.ShaderManager.UpdateFloat("time", (float)Clock.LastCurrent);
            Renderer.ShaderManager.UpdateMatrix4("parentTransform", TotalTransform);

            //Generate Models with Compute Shader
            GL.DispatchCompute(1, 1, 1);

            //Now Render them!
            Renderer.ShaderManager.ActiveShaderProgram = render;
            render.SetActive();

            //Update Uniforms
            Renderer.ShaderManager.UpdateVector2("size", texture.Size);
            Renderer.CurrentContext.BindTexture(texture);

            for (int i = 0; i < ParticleManager.Particles.Values.Length; i++)
            {
                Renderer.ShaderManager.UpdateInt("index", i);
                Renderer.CurrentContext.RenderSpriteQuad();
            }

            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
            Renderer.SpriteProgram.SetActive();

            p.Record();
        }

        public override void Add(IDrawable2D child, AddPosition position = AddPosition.Last) => 
            Debugger.InvalidOperation($"Don't Add to {nameof(ParticleLayer)}");

        public override void Remove(IDrawable2D child, bool dispose = true) => 
            Debugger.InvalidOperation($"Don't Remove from {nameof(ParticleLayer)}");

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Logger.Benchmark(p);

            compute.Dispose();
            render.Dispose();
        }
    }
}