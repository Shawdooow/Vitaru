// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Golgi.Graphics.Overlays;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders.Structs;
using Prion.Mitochondria.Graphics.Contexts.GL46.SSBOs;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Lights;
using Prion.Mitochondria.Graphics.Models;
using Prion.Mitochondria.Graphics.Models.Meshes;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Input;

namespace Vitaru.Mods.Included
{
    public class Tanks : Mod
    {
        public override bool Disabled => !Renderer._3D_AVAILABLE || Vitaru.FEATURES < Features.Upcoming;

        public override Button GetMenuButton() =>
            new Button
            {
                Y = -60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.MediumSlateBlue
                },

                Text = "Tanks"
            };

        public override Root GetRoot() => new TanksRoot();

        private class TanksRoot : ExitableRoot
        {
            public override string Name => nameof(TanksRoot);

            private Camera camera;
            private PlayerBinds input;
            private TexturedModel turret;

            private LightPointer global;
            private LightPointer red;
            private LightPointer green;

            public TanksRoot()
            {
                input = new PlayerBinds();
            }

            public override void LoadingComplete()
            {
                camera = new Camera();

                LightManager.SetSSBO(new SSBO<Light>(1));

                global = LightManager.GetLight();
                global.Position = new Vector3(0, -200, -100);
                global.Diffuse = Color.BurlyWood.Vector();

                red = LightManager.GetLight();
                red.Position = new Vector3(-50, -200, 0);
                red.Diffuse = Color.Red.Vector();

                green = LightManager.GetLight();
                green.Position = new Vector3(50, -200, 0);
                green.Diffuse = Color.GreenYellow.Vector();

                const float scale = 0.05f;

                TexturedModel body = new TexturedModel
                {
                    Scale = new Vector3(scale)
                };
                body.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("tank body.obj")));
                Renderer.CurrentContext.BufferMeshes(body);

                turret = new TexturedModel
                {
                    Scale = new Vector3(scale)
                };
                turret.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("tank turret.obj")));
                Renderer.CurrentContext.BufferMeshes(turret);

                Add(new Layer3D<TexturedModel>
                {
                    //TODO: make this work Scale = new Vector3(0.05f),

                    Children = new[]
                    {
                        body,
                        turret
                    }
                });

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.7f,
                    Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));

                Add(new PerformanceDisplay(DisplayType.FPS));

                base.LoadingComplete();
            }

            private float speed = 5;
            private double dx, dy, lx, ly;
            private MouseState m;

            public override void Update()
            {
                base.Update();

                if (Renderer.Window.Focused)
                {
                    m = Mouse.GetCursorState();
                    dx = lx - m.X;
                    dy = ly - m.Y;

                    float t = (float) Clock.LastElapsedTime / 1000f;
                    t *= speed;

                    if (input[VitaruActions.Up])
                        camera.Position += camera.Front * t;
                    else if (input[VitaruActions.Down])
                        camera.Position -= camera.Front * t;

                    if (input[VitaruActions.Right])
                        camera.Position += camera.Right * t;
                    else if (input[VitaruActions.Left])
                        camera.Position -= camera.Right * t;

                    if (input[VitaruActions.Jump])
                        camera.Position += camera.Up * t;
                    else if (input[VitaruActions.Sneak])
                        camera.Position -= camera.Up * t;

                    mouseInput();

                    Mouse.SetPosition(1920f / 2, 1080f / 2);
                    lx = 1920f / 2;
                    ly = 1080f / 2;
                }
            }

            private void mouseInput()
            {
                const float sens = 0.002f;
                camera.Rotation += new Vector3(0, (float) dy * sens, -(float) dx * sens);
            }

            public override void PreRender()
            {
                base.PreRender();

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                LightManager.UpdateLights();
                LightManager.UpdateSSBO();

                Matrix4x4 m = Matrix4x4.CreateScale(new Vector3(
                    (float) Math.Sin(DrawClock.Current / 1000f * speed) * 0.5f + 1f,
                    (float) Math.Cos(DrawClock.Current / 1000f * speed) * 0.5f + 1f, 1));

                m *= Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0),
                    (float) (DrawClock.Current / 1000d) * speed);

                Renderer.ShaderManager.UpdateMatrix4("view", camera.View);
                Renderer.ShaderManager.UpdateMatrix4("model", m);
            }

            protected override void Dispose(bool finalize)
            {
                base.Dispose(finalize);

                if (global == null) return;

                LightManager.ReturnLight(green);
                LightManager.ReturnLight(red);
                LightManager.ReturnLight(global);
            }
        }
    }
}