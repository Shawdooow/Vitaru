// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Graphics.Particles;
using Vitaru.Play;
using Vitaru.Settings;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class PlayTest : PlayRoot
    {
        public override string Name => nameof(PlayTest);

        private readonly Gamefield gamefield;

        private readonly SpriteText enemies;
        private readonly SpriteText bullets;
        private readonly SpriteText particles;

        private readonly SpriteText timeIn;
        private readonly Slider slider;
        private readonly SpriteText timeLeft;

        private readonly bool multithread = Vitaru.VitaruSettings.GetBool(VitaruSetting.ThreadTranforms);

        private int start;
        private int end;

        public PlayTest()
        {
            gamefield = new Gamefield
            {
                Clock = TrackManager.CurrentTrack.Clock,
                OnShadeChange = shade => ShadeLayer.Shade = shade,
                OnIntensityChange = intensity => ShadeLayer.Intensity = intensity
            };

            Player player = new Sakuya(gamefield);

            gamefield.Add(player);

            //Packs
            Add(gamefield);

            //Layers
            Add(gamefield.CharacterLayer);
            Add(gamefield.ParticleLayer);
            Add(gamefield.ProjectilesLayer);

            Add(enemies = new SpriteText
            {
                Position = new Vector2(-2, 2),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f
            });
            Add(bullets = new SpriteText
            {
                Position = new Vector2(-2, 20),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f
            });
            Add(particles = new SpriteText
            {
                Position = new Vector2(-2, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f
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
                        TrackManager.CurrentTrack.Seek(PrionMath.Scale(p, 0, 1, 0, TrackManager.CurrentTrack.Length))
                }
            });

            slider.AddArray(new IDrawable2D[]
            {
                timeIn = new SpriteText
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.TopLeft,
                    Position = new Vector2(8),
                    TextScale = 0.25f
                },
                timeLeft = new SpriteText
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.TopRight,
                    Position = new Vector2(-8, 8),
                    TextScale = 0.25f
                }
            });
        }

        private void enemy()
        {
            gamefield.Add(new Enemy(gamefield)
            {
                StartTime = TrackManager.CurrentTrack.Clock.LastCurrent,
                StartPosition = new Vector2(PrionMath.RandomNumber(-200, 200), PrionMath.RandomNumber(-300, 0)),
                OnDie = enemy
            });
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            enemy();
            Game.TextureStore.GetTexture("particle.png");
        }

        public override void Update()
        {
            enemies.Text = $"{Enemy.COUNT} Enemies";
            bullets.Text = $"{Bullet.COUNT} Bullets";
            particles.Text = $"{Particle.COUNT} Particles";

            TrackManager.CurrentTrack.Clock.Update();

            float current = (float) TrackManager.CurrentTrack.Clock.Current;
            float length = (float) TrackManager.CurrentTrack.Length * 1000;

            if (!slider.Dragging)
                slider.Progress = PrionMath.Scale(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            timeLeft.Text = left;

            TrackManager.TryRepeatTrack();
            if (TrackManager.CurrentTrack.CheckNewBeat())
            {
                for (int i = 0; i < gamefield.PlayerPack.Children.Count; i++)
                    gamefield.PlayerPack.Children[i].OnNewBeat();

                for (int i = 0; i < gamefield.LoadedEnemies.Children.Count; i++)
                    gamefield.LoadedEnemies.Children[i].OnNewBeat();
            }

            while (que > 0)
            {
                que--;
                enemy();
            }

            base.Update();
        }

        protected override void UpdateTransforms()
        {
            if (multithread && Transforms.Count >= 500)
            {
                assignIndexes();
                Vitaru.RunThreads();
                proccessTransforms(start, end);
                Vitaru.AwaitDynamicThreads();
            }
            else
                base.UpdateTransforms();
        }

        private void proccessTransforms(int s, int e)
        {
            for (int i = s; i < e; i++)
                Transforms[i].Update();
        }

        private void assignIndexes()
        {
            int st = 0;
            int en = 0;

            int tcount = Transforms.Count;
            int dcount = Vitaru.DynamicThreads.Count;

            float ratio = (float) tcount / dcount;
            int remainder = tcount % dcount;

            int iter = (int) Math.Round(ratio, MidpointRounding.ToZero);

            for (int i = 0; i < dcount; i++)
            {
                en += iter;

                int s = st;
                int e = en;

                Vitaru.DynamicThreads[i].Task = () => proccessTransforms(s, e);
                st = en + 1;
            }

            start = st;
            end = en + remainder;
        }

        public override void PreRender()
        {
            base.PreRender();
            gamefield.PreRender();
        }

        private int que;

        protected override void OnKeyDown(KeyboardKeyEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Keys.R:
                    que = 1;
                    break;
                case Keys.T:
                    que = 10;
                    break;
            }
        }
    }
}