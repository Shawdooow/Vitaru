// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime;
using OpenTK.Graphics.OpenGL;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Graphics.Overlays;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Cameras;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
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
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Tracks;
#if !PUBLISH || PERSONAL
using ShaderType = Prion.Mitochondria.Graphics.Shaders.ShaderType;

#endif

namespace Vitaru.Play.Mods.Included
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

            protected override GCLatencyMode GCLatencyMode => GCLatencyMode.SustainedLowLatency;

            private VitaruTrackController controller;

            private Camera camera;
            private PlayerBinds input;
            private Model turret;
            private BillboardSprite bill;

            private const float walking_speed = 1.34f;

#if !PUBLISH || PERSONAL
            private Model starship;
#endif

            //private SnowLayer snow;

            private LightPointer global;
            private LightPointer torch;
            private LightPointer blue;
            private LightPointer red;

            private Text2D position;
#if !PUBLISH || PERSONAL
            private Text2D mission;
            private Text2D altitude;
            private Text2D velocity;

            private GLShaderProgram vNormal;
            private GLShaderProgram fNormal;
#endif

            private bool flashlight = true;
            private bool lighthouses = true;
            private bool left;
            private bool normal;
            private bool wireframe;

#if !PUBLISH || PERSONAL
            private int launch;
            private double time = -10.6f;

            private const float max_altitude = 10000;
            private const double time_to_max_altitude = 260d;

            private const double rise_to_1km = 40d;
            private const double rise_to_4km = 114d;
            private const double rise_to_10km = 237d;

            private const double fall_to_6km = 293d;
            private const double fall_to_2km = 333d;
            private const double fall_to_1km = 344d;

            private Sound flight;
            private LightPointer raptor1; // 0, 4, 3
            private LightPointer raptor2; // -3, 4, -3
            private LightPointer raptor3; // 3, 4, -3

            private Vector3 savedPos;
            private Vector3 savedRot;
            private bool ride;

            public override void PreLoading()
            {
                base.PreLoading();

                Game.SampleStore.GetSample("SN10 Flight.wav");
            }
#endif

            public override void LoadingComplete()
            {
#if !PUBLISH || PERSONAL
                flight = new Sound(new SeekableClock(), Game.SampleStore.GetSample("SN10 Flight.wav"))
                {
                    Gain = 100,
                    Rolloff = 0
                };

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

                Add(controller = new VitaruTrackController
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

                Model world = new()
                {
                    Position = new Vector3(0, 10, 0),
                    Scale = new Vector3(0.01f),
                    Yaw = MathF.PI
                };
                world.Add(new Mesh(Game.MeshStore.GetVertecies("Alki Demo World 4 SD.obj")));
                Renderer.Context.BufferMeshes(world);

#if !PUBLISH || PERSONAL
                starship = new Model
                {
                    Position = new Vector3(0, -2, -20),
                    Yaw = MathF.PI
                };
                starship.Add(new Mesh(Game.MeshStore.GetVertecies("SN10.obj")));
                Renderer.Context.BufferMeshes(starship);
#endif

                Model body = new()
                {
                    Scale = new Vector3(scale),
                    Yaw = MathF.PI
                };
                body.Add(new Mesh(Game.MeshStore.GetVertecies("tank body.obj")));
                Renderer.Context.BufferMeshes(body);

                turret = new Model
                {
                    Scale = new Vector3(scale),
                    Yaw = MathF.PI
                };
                turret.Add(new Mesh(Game.MeshStore.GetVertecies("tank turret.obj")));
                Renderer.Context.BufferMeshes(turret);

                Model left = new()
                {
                    Position = TrackManager.CurrentTrack.Source.LeftPosition,
                    Scale = new Vector3(scale * 4),
                    Color = Color.Blue
                };
                left.Add(new Mesh(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(left);

                Model right = new()
                {
                    Position = TrackManager.CurrentTrack.Source.RightPosition,
                    Scale = new Vector3(scale * 4),
                    Color = Color.Red
                };
                right.Add(new Mesh(Game.MeshStore.GetVertecies("sphere.obj")));
                Renderer.Context.BufferMeshes(right);

                bill = new BillboardSprite(camera)
                {
                    Position = new Vector3(0, 4, 0)
                };
                Renderer.Context.BufferMeshes(bill);

                Add(new Layer3D<Model>
                {
                    //TODO: make this work Scale = new Vector3(0.05f),

                    Children = new[]
                    {
                        world,
#if !PUBLISH || PERSONAL
                        starship,
#endif
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
                Add(position = new Text2D
                {
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.TopRight,
                    FontScale = 0.25f
                });
#if !PUBLISH || PERSONAL
                Add(mission = new Text2D
                {
                    Position = new Vector2(-120, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopLeft,
                    Alpha = 0,
                    Text = "T-10.6"
                });
                Add(altitude = new Text2D
                {
                    Position = new Vector2(120, 80),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopRight,
                    FontScale = 0.5f,
                    Alpha = 0,
                    Text = "0 Meters"
                });
                Add(velocity = new Text2D
                {
                    Position = new Vector2(100, 120),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopRight,
                    FontScale = 0.5f,
                    Alpha = 0,
                    Text = "0 m/s"
                });
#endif

                Add(new Layer2D<IDrawable2D>
                {
                    Child = new Circle
                    {
                        Size = new Vector2(6)
                    }
                });

#if !PUBLISH || PERSONAL
                flight.Position = starship.Position;

                raptor1 = LightManager.GetLight();
                raptor1.Position = starship.Position + new Vector3(0, 4, 2.5f);
                raptor1.Diffuse = Color.Black.Vector();
                raptor1.Falloffs = new Vector3(0.01f);

                raptor2 = LightManager.GetLight();
                raptor2.Position = starship.Position + new Vector3(-2.5f, 4, -2.5f);
                raptor2.Diffuse = Color.Black.Vector();
                raptor2.Falloffs = new Vector3(0.01f);

                raptor3 = LightManager.GetLight();
                raptor3.Position = starship.Position + new Vector3(2.5f, 4, -2.5f);
                raptor3.Diffuse = Color.Black.Vector();
                raptor3.Falloffs = new Vector3(0.01f);
#endif

                base.LoadingComplete();
            }

            private int w = 1920 / 2;
            private int h = 1080 / 2;

            private float deltaX;
            private float deltaY;

#if !PUBLISH || PERSONAL
            private double flicker1, flicker2, flicker3;
#endif

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

#if !PUBLISH || PERSONAL
                if (launch >= 4)
                {
                    time += Clock.LastElapsedTime / 1000;

                    mission.Text = time < 0 ? $"T{Math.Round(time, 1)}" : $"T+{Math.Round(time, 1)}";

                    float alt = starship.Y + 2f;
                    altitude.Text = alt >= 1000 ? $"{Math.Round(alt / 1000, 1)} KM" : $"{Math.Round(alt, 1)} Meters";

                    //Engine startup
                    if (launch == 4 && time >= -0.6d)
                    {
                        launch++;

                        new Vector3Transform(value => raptor1.Diffuse = value, raptor1.Diffuse,
                            Color.LightSalmon.Vector(), this, Clock.Current, 170, Easings.None)
                        {
                            Name = "Raptor 1"
                        };
                        new Vector3Transform(value => raptor2.Diffuse = value, raptor2.Diffuse,
                            Color.LightSalmon.Vector(), this, Clock.Current, 180, Easings.None)
                        {
                            Name = "Raptor 2"
                        };
                        new Vector3Transform(value => raptor3.Diffuse = value, raptor3.Diffuse,
                            Color.LightSalmon.Vector(), this, Clock.Current, 190, Easings.None)
                        {
                            Name = "Raptor 3"
                        };
                    }

                    //Actual launch time
                    if (launch >= 5 && time >= 0)
                        launch++;

                    if (launch >= 6)
                    {
                        Vector3 old = starship.Position;
                        starship.Position = getStarshipPosition(time) + new Vector3(0, -2, -20);
                        if (ride) camera.Position += starship.Position - old;

                        flight.Position = starship.Position;
                        raptor1.Position = starship.Position + new Vector3(0, 4, 3);
                        raptor2.Position = starship.Position + new Vector3(-3, 4, -3);
                        raptor3.Position = starship.Position + new Vector3(3, 4, -3);

                        velocity.Text = $"{Math.Round((starship.Y - old.Y) * (1000 / Clock.LastElapsedTime), 1)} m/s";

                        if (Clock.LastCurrent > flicker1)
                        {
                            int length = PrionMath.RandomNumber(200, 400);
                            flicker1 = Clock.LastCurrent + length;
                            raptor1.Falloffs = new Vector3(0.009f);

                            new Vector3Transform(value => raptor1.Falloffs = value, raptor1.Falloffs,
                                new Vector3(0.01f), this, Clock.Current, length * 0.8f, Easings.None)
                            {
                                Name = "Raptor 1"
                            };
                        }

                        if (Clock.LastCurrent > flicker2)
                        {
                            int length = PrionMath.RandomNumber(200, 400);
                            flicker2 = Clock.LastCurrent + length;
                            raptor2.Falloffs = new Vector3(0.009f);

                            new Vector3Transform(value => raptor2.Falloffs = value, raptor2.Falloffs,
                                new Vector3(0.01f), this, Clock.Current, length * 0.8f, Easings.None)
                            {
                                Name = "Raptor 2"
                            };
                        }

                        if (Clock.LastCurrent > flicker3)
                        {
                            int length = PrionMath.RandomNumber(200, 400);
                            flicker3 = Clock.LastCurrent + length;
                            raptor3.Falloffs = new Vector3(0.009f);

                            new Vector3Transform(value => raptor3.Falloffs = value, raptor3.Falloffs,
                                new Vector3(0.01f), this, Clock.Current, length * 0.8f, Easings.None)
                            {
                                Name = "Raptor 3"
                            };
                        }
                    }
                }
#endif

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
                    t *= walking_speed;

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
                    dim, this, Clock.Current, TrackManager.CurrentTrack.Metadata.GetBeatLength() * 0.8f, Easings.None)
                {
                    Name = "Blue"
                };
            }

            private void flashRight()
            {
                red.Falloffs = new Vector3(0.1f);

                new Vector3Transform(value => red.Falloffs = value, red.Falloffs,
                    dim, this, Clock.Current, TrackManager.CurrentTrack.Metadata.GetBeatLength() * 0.8f, Easings.None)
                {
                    Name = "Red"
                };
            }

#if !PUBLISH || PERSONAL
            private Vector3 getStarshipPosition(double time)
            {
                double y;
                double z;

                Vector3 position = Vector3.Zero;

                switch (time)
                {
                    case >= 0 and < rise_to_1km:
                        y = Easing.ApplyEasing(Easings.InSine, PrionMath.Remap(time, 0, rise_to_1km));

                        position.Y = PrionMath.Remap((float) y, 0, 1, 0, 1000);
                        break;
                    case >= rise_to_1km and < rise_to_4km:
                        z = Easing.ApplyEasing(Easings.InSine, PrionMath.Remap(time, rise_to_1km, rise_to_4km));
                        y = Easing.ApplyEasing(Easings.None, PrionMath.Remap(time, rise_to_1km, rise_to_4km));

                        position.Y = PrionMath.Remap((float) y, 0, 1, 1000, 4000);
                        position.Z = PrionMath.Remap((float) z, 0, 1, 0, -100);
                        break;
                    case >= rise_to_4km and < rise_to_10km:
                        z = Easing.ApplyEasing(Easings.OutSine, PrionMath.Remap(time, rise_to_4km, rise_to_10km));
                        y = Easing.ApplyEasing(Easings.None, PrionMath.Remap(time, rise_to_4km, rise_to_10km));

                        position.Y = PrionMath.Remap((float) y, 0, 1, 4000, 10000);
                        position.Z = PrionMath.Remap((float) z, 0, 1, -100, -500);
                        break;
                }

                return position;
            }
#endif

            public override void PreRender()
            {
                base.PreRender();

                Renderer.TextureProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.TextureProgram;

                LightManager.UpdateLights();
                LightManager.UpdateShaderStorageBuffer();

                Matrix4x4 m = Matrix4x4.CreateScale(new Vector3(
                    (float) Math.Sin(DrawClock.Current / 1000f * walking_speed) * 0.5f + 1f,
                    (float) Math.Cos(DrawClock.Current / 1000f * walking_speed) * 0.5f + 1f, 1));

                m *= Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0),
                    (float) (DrawClock.Current / 1000d) * walking_speed);

                Renderer.ShaderManager.UpdateMatrix4("view", camera.View);
                Renderer.ShaderManager.UpdateMatrix4("model", m);

#if !PUBLISH || PERSONAL
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

#if !PUBLISH || PERSONAL
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
#if !PUBLISH || PERSONAL
                    case Keys.Q:
                        if (!ride)
                        {
                            ride = true;

                            Vector3 rot = camera.Rotation;
                            camera.Rotation = savedRot;
                            savedRot = rot;

                            savedPos = camera.Position;

                            camera.Position = starship.Position + new Vector3(0, 48, 2);
                        }
                        else
                        {
                            ride = false;

                            Vector3 rot = camera.Rotation;
                            camera.Rotation = savedRot;
                            savedRot = rot;

                            camera.Position = savedPos;
                        }

                        break;
#endif
                }

#if !PUBLISH || PERSONAL
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
                        altitude.Alpha = 1;
                        velocity.Alpha = 1;
                        flight.Play();
                        break;
                }
#endif
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

#if !PUBLISH || PERSONAL
                flight.Dispose();

                fNormal.Dispose();
                vNormal.Dispose();

                LightManager.ReturnLight(raptor3);
                LightManager.ReturnLight(raptor2);
                LightManager.ReturnLight(raptor1);
#endif
                LightManager.ReturnLight(red);
                LightManager.ReturnLight(blue);
                LightManager.ReturnLight(torch);
                LightManager.ReturnLight(global);
            }
        }
    }
}