// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two;
using Vitaru.Graphics;
using Vitaru.Levels;
using Vitaru.Play;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;
using Vitaru.Settings;

namespace Vitaru.Roots.Tests
{
    public class PlayTest : PlayRoot
    {
        public override string Name => nameof(PlayTest);

        private readonly int particle_cap = Vitaru.VitaruSettings.GetInt(VitaruSetting.ParticleCap);
        private readonly int bullet_cap = Vitaru.VitaruSettings.GetInt(VitaruSetting.BulletCap);

        private readonly Gamefield gamefield;

        private readonly bool random;
        private double spiral;

        private readonly Text2D enemies;
        private readonly Text2D bullets;
        private readonly Text2D particles;

        private readonly Text2D timeIn;
        private readonly Slider slider;
        private readonly Text2D timeLeft;

        private readonly SurroundSoundVisualizer surround;

        public PlayTest()
        {
            gamefield = new Gamefield
            {
                Clock = TrackManager.CurrentTrack.Clock,
            };

            if (gamefield.UnloadedEnemies.Count <= 0 || LevelStore.UseRandom)
                random = true;

            Player player = GamemodeStore.SelectedGamemode.SelectedCharacter != string.Empty
                ? GamemodeStore.GetPlayer(GamemodeStore.SelectedGamemode.SelectedCharacter, gamefield)
                : new Yuie(gamefield);

            gamefield.Add(player);
            gamefield.SetPlayer(player);

            //Packs
            Add(gamefield);

            //Layers
            Add(gamefield.ParticleLayer);
            Add(gamefield.CharacterLayer);
            Add(gamefield.BulletLayer);
            Add(gamefield.Border);
            Add(gamefield.OverlaysLayer);

            Add(enemies = new Text2D(false)
            {
                Position = new Vector2(-2, 2),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                FontScale = 0.25f,
            });
            Add(bullets = new Text2D(false)
            {
                Position = new Vector2(-2, 20),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                FontScale = 0.25f,
            });
            Add(particles = new Text2D(false)
            {
                Position = new Vector2(-2, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                FontScale = 0.25f,
            });

            //Intentional, as we dont want the slider to receive input...
            Add(new InputLayer<Slider>
            {
                ParentOrigin = Mounts.TopCenter,
                Origin = Mounts.TopCenter,
                PassDownInput = false,

                Child = slider = new Slider
                {
                    Y = 8,
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    Width = 800,
                    OnProgressInput = p =>
                        TrackManager.CurrentTrack.Seek(PrionMath.Remap(p, 0, 1, 0,
                            TrackManager.CurrentTrack.Sample.Length)),
                },
            });

            slider.AddArray(new IDrawable2D[]
            {
                timeIn = new Text2D(false)
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.TopLeft,
                    Position = new Vector2(8),
                    FontScale = 0.25f,
                },
                timeLeft = new Text2D(false)
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.TopRight,
                    Position = new Vector2(-8, 8),
                    FontScale = 0.25f,
                },
            });

            TrackManager.CurrentTrack.Gain *= 2f;
            TrackManager.CurrentTrack.Rolloff = 0.002f;
            TrackManager.CurrentTrack.StereoDistance = new Vector3(1600, 0, 0);
            TrackManager.CurrentTrack.Position = new Vector3(0, 0, -400);

            Add(surround = new SurroundSoundVisualizer());
        }

        public override void Update()
        {
            enemies.Text = $"{Enemy.COUNT} Enemies";
            bullets.Text = $"{Bullet.COUNT} Bullets (Max Drawables = {bullet_cap})";
            particles.Text = $"Max Particles = {particle_cap}";

            TrackManager.CurrentTrack.Clock.Update();

            float current = (float)TrackManager.CurrentTrack.Clock.Current;
            float length = (float)TrackManager.CurrentTrack.Sample.Length * 1000;

            if (!slider.Dragging)
                slider.Progress = PrionMath.Remap(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            timeLeft.Text = left;

            TrackManager.TryRepeatTrack();
            if (TrackManager.CurrentTrack.CheckNewBeat())
            {
                if (random && TrackManager.CurrentTrack.Clock.Current >= spiral)
                {
                    int count = PrionMath.RandomNumber(1, 5);
                    double start = TrackManager.CurrentTrack.Clock.Current +
                                   TrackManager.CurrentTrack.Metadata.GetBeatLength() * 2;

                    for (int i = 0; i < count; i++)
                    {
                        Color c = Background.Texture.GetPixel(
                            PrionMath.RandomNumber(0, (int)Background.Texture.Size.X),
                            PrionMath.RandomNumber(0, (int)Background.Texture.Size.Y));

                        bool s = count == 1 && PrionMath.RandomNumber(0, 5) == 2;

                        if (!s)
                            gamefield.Add(new Enemy(gamefield)
                            {
                                StartTime = start,
                                StartPosition = new Vector2(PrionMath.RandomNumber(-200, 200),
                                    PrionMath.RandomNumber(-300, 0)),
                                PatternID = (short)PrionMath.RandomNumber(0, 5),
                                Color = c,
                            });
                        else
                        {
                            spiral = start + TrackManager.CurrentTrack.Metadata.GetBeatLength() * 4;
                            Enemy e = new(gamefield)
                            {
                                StartTime = start,
                                EndTime = spiral,
                                StartPosition = Vector2.Zero,
                                MaxHealth = 120,
                                PatternID = 5,
                                Color = c,
                            };
                            gamefield.Add(e);
                        }
                    }
                }

                surround.OnNewBeat();

                for (int i = 0; i < gamefield.PlayerPack.Children.Count; i++)
                    gamefield.PlayerPack.Children[i].OnNewBeat();

                for (int i = 0; i < gamefield.LoadedEnemies.Children.Count; i++)
                    gamefield.LoadedEnemies.Children[i].OnNewBeat();
            }

            base.Update();
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            TrackManager.CurrentTrack.Gain /= 2f;
        }

        public override void PreRender()
        {
            base.PreRender();
            gamefield.PreRender();
        }

        public override void PostProcessing()
        {
            Renderer.ShaderManager.UpdateInt("shade", (int)gamefield.Shade);
            Renderer.ShaderManager.UpdateFloat("intensity", gamefield.Intensity);
        }

        protected override void Dispose(bool finalize)
        {
            Renderer.ShaderManager.UpdateInt("shade", (int)Shades.Color);
            Renderer.ShaderManager.UpdateFloat("intensity", 1);
            base.Dispose(finalize);
        }
    }
}