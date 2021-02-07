// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Graphics.Overlays;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders.Structs;
using Prion.Mitochondria.Graphics.Contexts.GL46.Vertices;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Layers._3D;
using Prion.Mitochondria.Graphics.Lights;
using Prion.Mitochondria.Graphics.Models;
using Prion.Mitochondria.Graphics.Models.Meshes;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Tracks;

namespace Vitaru.Mods.Included
{
    public class Tanks : Mod
    {
        public override bool Disabled => !Renderer._3D_AVAILABLE || Vitaru.FEATURES < Features.Experimental;

        public override Button GetMenuButton() =>
            new()
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

            //private SnowLayer snow;

            private LightPointer global;
            private LightPointer blue;
            private LightPointer red;

            public override void LoadingComplete()
            {
                input = new PlayerBinds();
                TrackManager.CurrentTrack.Position = new Vector3(0, 2, -2);

                camera = new Camera();
                InputManager.Translator.SetMousePosition(1920 / 2, 1080 / 2);

                LightManager.SetShaderStorageBuffer(new ShaderStorageBuffer<Light>(1));

                global = LightManager.GetLight();
                global.Position = new Vector3(0, -200, -100);
                global.Diffuse = Color.BurlyWood.Vector();

                blue = LightManager.GetLight();
                blue.Position = TrackManager.CurrentTrack.Source.LeftPosition.NegativeXZ();
                blue.Diffuse = Color.Blue.Vector();

                red = LightManager.GetLight();
                red.Position = TrackManager.CurrentTrack.Source.RightPosition.NegativeXZ();
                red.Diffuse = Color.Red.Vector();

                const float scale = 0.05f;

                TexturedModel body = new()
                {
                    Scale = new Vector3(scale),
                    Yaw = MathF.PI
                };
                body.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("tank body.obj")));
                Renderer.Context.BufferMeshes(body);

                turret = new TexturedModel
                {
                    Scale = new Vector3(scale),
                    Yaw = MathF.PI
                };
                turret.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("tank turret.obj")));
                Renderer.Context.BufferMeshes(turret);

                TexturedModel left = new()
                {
                    Position = TrackManager.CurrentTrack.Source.LeftPosition,
                    Scale = new Vector3(scale),
                    Color = Color.Blue
                };
                left.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(left);

                TexturedModel right = new()
                {
                    Position = TrackManager.CurrentTrack.Source.RightPosition,
                    Scale = new Vector3(scale),
                    Color = Color.Red
                };
                right.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(right);

                Add(new Layer3D<TexturedModel>
                {
                    //TODO: make this work Scale = new Vector3(0.05f),

                    Children = new[]
                    {
                        body,
                        turret,
                        left,
                        right
                    }
                });

                //Add(snow = new SnowLayer());

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.7f,
                    Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));

                Add(new PerformanceDisplay(DisplayType.FPS));

                Add(new Layer2D<IDrawable2D>
                {
                    Child = new Circle
                    {
                        Size = new Vector2(6)
                    }
                });

                base.LoadingComplete();
            }

            private float speed = 5;

            private int w = 1920 / 2;
            private int h = 1080 / 2;

            private float deltaX;
            private float deltaY;

            //private double s;

            public override void Update()
            {
                base.Update();

                //s += Clock.LastElapsedTime;

                //if (s >= 5)
                //{
                //    s = 0;
                //
                //    Vector3 start = new Vector3(PrionMath.RandomNumber(-50, 50), PrionMath.RandomNumber(-50, 50), 50);
                //    Vector3 end = start -
                //                  new Vector3(PrionMath.RandomNumber(-5, 5), PrionMath.RandomNumber(-5, 5), 100) -
                //                  new Vector3(10, 10, 0);
                //
                //    snow.Add(new SnowParticle
                //    {
                //        StartPosition = start,
                //        EndPosition = end,
                //        Alpha = 1,
                //        Scale = 1f // / PrionMath.RandomNumber(1, 5)
                //    });
                //}

                //snow.UpdateParticles(0, 8192, (float) Clock.LastElapsedTime);

                if (Renderer.Window.Focused)
                {
                    Vector2 m = InputManager.Mouse.ScreenPosition;
                    deltaX = w - m.X;
                    deltaY = h - m.Y;

                    mouseInput();

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

                    InputManager.Translator.SetMousePosition(1920 / 2, 1080 / 2);

                    AudioManager.Context.Listener.Position = camera.Position;
                    AudioManager.Context.Listener.Direction = camera.Front;
                }
            }

            private void mouseInput()
            {
                const float sens = 0.002f;
                camera.Rotation += new Vector3(0, deltaY * sens, -deltaX * sens);
            }

            public override void PreRender()
            {
                base.PreRender();

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                LightManager.UpdateLights();
                LightManager.UpdateShaderStorageBuffer();

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

                LightManager.ReturnLight(red);
                LightManager.ReturnLight(blue);
                LightManager.ReturnLight(global);
            }
        }
    }
}