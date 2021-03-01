// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#define NORMAL

using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;
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
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Utilities;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Tracks;
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

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

            private TrackController controller;

            private Camera camera;
            private PlayerBinds input;
            private TexturedModel turret;
            private BillboardSprite bill;

            //private SnowLayer snow;

            private LightPointer global;
            private LightPointer torch;
            private LightPointer blue;
            private LightPointer red;

            private InstancedText position;
            private InstancedText mission;

#if NORMAL
            private GLShaderProgram vNormal;
            private GLShaderProgram fNormal;
#endif

            private bool flashlight = true;
            private bool lighthouses = true;
            private bool left;
            private bool normal;
            private bool wireframe;

            private int launch;
            private double time = -10;

            public override void LoadingComplete()
            {
#if NORMAL
                string v = new StreamReader(Game.ShaderStorage.GetStream("Debug\\vNormal.vert")).ReadToEnd();
                string g = new StreamReader(Game.ShaderStorage.GetStream("Debug\\vNormal.geom")).ReadToEnd();
                string f = new StreamReader(Game.ShaderStorage.GetStream("Debug\\vNormal.frag")).ReadToEnd();

                Shader vert = new GLShader(ShaderType.Vertex, "VertexNormal", v);
                Shader geom = new GLShader(ShaderType.Geometry, "VertexNormal", g);
                Shader frag = new GLShader(ShaderType.Pixel, "VertexNormal", f);

                vNormal = new GLShaderProgram(vert, geom, frag)
                {
                    Name = "VertexNormal"
                };

                //Vertex
                vNormal.Locations["projection"] = GLShaderManager.GetLocation(vNormal, "projection");
                vNormal.Locations["model"] = GLShaderManager.GetLocation(vNormal, "model");
                vNormal.Locations["view"] = GLShaderManager.GetLocation(vNormal, "view");

                //Pixel
                vNormal.Locations["modelColor"] = GLShaderManager.GetLocation(vNormal, "modelColor");

                Renderer.OnResize += value =>
                {
                    vNormal.SetActive();
                    Renderer.ShaderManager.ActiveShaderProgram = vNormal;
                    Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.9f,
                        Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));
                };

                vNormal.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = vNormal;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.9f,
                    Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));

                v = new StreamReader(Game.ShaderStorage.GetStream("Debug\\fNormal.vert")).ReadToEnd();
                g = new StreamReader(Game.ShaderStorage.GetStream("Debug\\fNormal.geom")).ReadToEnd();
                f = new StreamReader(Game.ShaderStorage.GetStream("Debug\\fNormal.frag")).ReadToEnd();

                vert = new GLShader(ShaderType.Vertex, "FaceNormal", v);
                geom = new GLShader(ShaderType.Geometry, "FaceNormal", g);
                frag = new GLShader(ShaderType.Pixel, "FaceNormal", f);

                fNormal = new GLShaderProgram(vert, geom, frag)
                {
                    Name = "FaceNormal"
                };

                //Vertex
                fNormal.Locations["projection"] = GLShaderManager.GetLocation(fNormal, "projection");
                fNormal.Locations["model"] = GLShaderManager.GetLocation(fNormal, "model");
                fNormal.Locations["view"] = GLShaderManager.GetLocation(fNormal, "view");

                //Pixel
                fNormal.Locations["modelColor"] = GLShaderManager.GetLocation(fNormal, "modelColor");

                Renderer.OnResize += value =>
                {
                    fNormal.SetActive();
                    Renderer.ShaderManager.ActiveShaderProgram = fNormal;
                    Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.9f,
                        Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));
                };

                fNormal.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = fNormal;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreatePerspectiveFieldOfView(0.9f,
                    Renderer.RenderWidth / (float) Renderer.RenderHeight, 0.1f, 100f));
#endif

                Add(controller = new TrackController
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    Position = new Vector2(20),
                    Alpha = 0.8f,
                    PassDownInput = false
                });

                input = new PlayerBinds();
                TrackManager.CurrentTrack.Position = new Vector3(0, 2, -2);

                camera = new Camera();
                InputManager.Translator.SetMousePosition(1920 / 2, 1080 / 2);

                LightManager.SetShaderStorageBuffer(new ShaderStorageBuffer<Light>(1));

                global = LightManager.GetLight();
                global.Position = new Vector3(100, -100, 500);
                global.Diffuse = new Vector3(0.8f, 0.9f, 1f) / 2;

                torch = LightManager.GetLight();
                torch.Diffuse = Color.DarkOrange.Vector();
                torch.Falloffs = new Vector3(0.5f, 0.5f, 0.05f);

                blue = LightManager.GetLight();
                blue.Position = TrackManager.CurrentTrack.Source.LeftPosition;
                blue.Diffuse = Color.Blue.Vector();
                blue.Falloffs = new Vector3(0.1f);

                red = LightManager.GetLight();
                red.Position = TrackManager.CurrentTrack.Source.RightPosition;
                red.Diffuse = Color.Red.Vector();
                red.Falloffs = new Vector3(0.1f);

                const float scale = 0.05f;

                TexturedModel world = new()
                {
                    Position = new Vector3(0, 10, 0),
                    Scale = new Vector3(0.01f),
                    Yaw = MathF.PI
                };
                world.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("Alki Demo World 4 SD.obj")));
                Renderer.Context.BufferMeshes(world);

                TexturedModel starship = new()
                {
                    Position = new Vector3(0, -2, -20),
                    Scale = new Vector3(1),
                    Yaw = MathF.PI
                };
                starship.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("SN10.obj")));
                Renderer.Context.BufferMeshes(starship);

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
                    Scale = new Vector3(scale * 4),
                    Color = Color.Blue
                };
                left.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(left);

                TexturedModel right = new()
                {
                    Position = TrackManager.CurrentTrack.Source.RightPosition,
                    Scale = new Vector3(scale * 4),
                    Color = Color.Red
                };
                right.Add(new Mesh<Vertex3Textured>(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(right);

                bill = new BillboardSprite(camera)
                {
                    Position = new Vector3(0, 4, 0)
                };
                Renderer.Context.BufferMeshes(bill);

                Add(new Layer3D<TexturedModel>
                {
                    //TODO: make this work Scale = new Vector3(0.05f),

                    Children = new[]
                    {
                        world,
                        starship,
                        body,
                        turret,
                        left,
                        right,
                        bill
                    }
                });

                //Add(snow = new SnowLayer());

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                Add(new PerformanceDisplay(DisplayType.FPS));
                Add(position = new InstancedText
                {
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.TopRight,
                    FontScale = 0.25f
                });
                Add(mission = new InstancedText
                {
                    Position = new Vector2(-80, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopLeft,
                    Alpha = 0,
                    Text = "T-10"
                });

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

                controller.Update();
                controller.TryRepeat();

                if (TrackManager.CurrentTrack.CheckNewBeat() && lighthouses)
                {
                    left = !left;

                    if (left)
                        flashLeft();
                    else
                        flashRight();
                }

                if (launch == 4)
                {
                    time += Clock.LastElapsedTime / 1000;
                    mission.Text = time < 0 ? $"T{Math.Round(time, 1)}" : $"T+{Math.Round(time, 1)}";
                }

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

                    torch.Position = camera.Position;
                    position.Text =
                        $"Position = (X = [{Math.Round(camera.Position.X, 2)}], Y = [{Math.Round(camera.Position.Y, 2)}], Z = [{Math.Round(camera.Position.Z, 2)}])";

                    InputManager.Translator.SetMousePosition(1920 / 2, 1080 / 2);

                    AudioManager.Context.Listener.Position = camera.Position;
                    AudioManager.Context.Listener.Direction = camera.Front;
                }

                bill.UpdateRotation();
            }

            private void mouseInput()
            {
                const float sens = 0.002f;
                camera.Rotation += new Vector3(0, deltaY * sens, -deltaX * sens);
            }

            private readonly Vector3 dim = new(0.5f);

            private void flashLeft()
            {
                blue.Falloffs = new Vector3(0.1f);

                new Vector3Transform(value => blue.Falloffs = value, blue.Falloffs,
                    dim, this, Clock.Current, TrackManager.CurrentTrack.Level.GetBeatLength() * 0.8f, Easings.None)
                {
                    Name = "Blue"
                };
            }

            private void flashRight()
            {
                red.Falloffs = new Vector3(0.1f);

                new Vector3Transform(value => red.Falloffs = value, red.Falloffs,
                    dim, this, Clock.Current, TrackManager.CurrentTrack.Level.GetBeatLength() * 0.8f, Easings.None)
                {
                    Name = "Red"
                };
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

#if NORMAL
                vNormal.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = vNormal;

                Renderer.ShaderManager.UpdateMatrix4("view", camera.View);
                Renderer.ShaderManager.UpdateMatrix4("model", m);

                fNormal.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = fNormal;

                Renderer.ShaderManager.UpdateMatrix4("view", camera.View);
                Renderer.ShaderManager.UpdateMatrix4("model", m);

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;
#endif
            }

#if NORMAL
            public override void Render3D()
            {
                if (wireframe) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                base.Render3D();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                if (normal)
                {
                    vNormal.SetActive();
                    Renderer.ShaderManager.ActiveShaderProgram = vNormal;

                    Layers3D.Render();

                    fNormal.SetActive();
                    Renderer.ShaderManager.ActiveShaderProgram = fNormal;

                    Layers3D.Render();

                    Renderer.TextureProgram.SetActive();
                    Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;
                }
            }
#endif

            protected override void OnKeyDown(KeyboardKeyEvent e)
            {
                base.OnKeyDown(e);

                switch (e.Key)
                {
                    case Keys.F:
                        flashlight = !flashlight;
                        torch.Diffuse = flashlight ? Color.DarkOrange.Vector() : Vector3.Zero;
                        break;
                    case Keys.R:
                        lighthouses = !lighthouses;
                        break;
                    case Keys.N:
                        normal = !normal;
                        break;
                    case Keys.M:
                        wireframe = !wireframe;
                        break;

                    case Keys.Z:
                        flashLeft();
                        break;
                    case Keys.X:
                        flashRight();
                        break;
                }

                switch (e.Key)
                {
                    default:
                        if (launch < 4) launch = 0;
                        break;
                    case Keys.T when launch == 0:
                        launch++;
                        break;
                    case Keys.Minus when launch == 1:
                        launch++;
                        break;
                    case Keys.One when launch == 2:
                        launch++;
                        break;
                    case Keys.Zero when launch == 3:
                        launch++;
                        mission.Alpha = 1;
                        break;
                }
            }

            protected override void OnMouseDown(MouseButtonEvent e)
            {
                base.OnMouseDown(e);

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        flashLeft();
                        break;
                    case MouseButtons.Right:
                        flashRight();
                        break;
                }
            }

            protected override void Dispose(bool finalize)
            {
                base.Dispose(finalize);

                if (global == null) return;

#if NORMAL
                fNormal.Dispose();
                vNormal.Dispose();
#endif

                LightManager.ReturnLight(torch);
                LightManager.ReturnLight(red);
                LightManager.ReturnLight(blue);
                LightManager.ReturnLight(global);
            }
        }
    }
}