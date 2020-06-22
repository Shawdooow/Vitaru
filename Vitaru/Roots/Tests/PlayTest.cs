// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Overlays;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class PlayTest : PlayRoot
    {
        private readonly Gamefield gamefield;
        private readonly SeekableClock seek;

        private readonly SpriteText debug;

        public PlayTest(SeekableClock seek)
        {
            this.seek = seek;

            gamefield = new Gamefield
            {
                Clock = seek,
                OnShadeChange = shade => ShadeLayer.Shade = shade,
                OnIntensityChange = intensity => ShadeLayer.Intensity = intensity
            };

            Player player = new Sakuya(gamefield);

            //Add(player.InputHandler);

            gamefield.Add(player);

            //Packs
            Add(gamefield);

            //Layers
            Add(gamefield.CharacterLayer);
            Add(gamefield.ParticleLayer);
            Add(gamefield.ProjectilesLayer);

            Add(new FPSOverlay());
            Add(debug = new SpriteText
            {
                Position = new Vector2(-2, 2),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f
            });
        }

        private void enemy()
        {
            gamefield.Add(new Enemy(gamefield)
            {
                StartTime = seek.LastCurrent,
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

        public static uint BULLET_COUNT;

        public override void Update()
        {
            debug.Text = $"{BULLET_COUNT}";
            seek.NewFrame();
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