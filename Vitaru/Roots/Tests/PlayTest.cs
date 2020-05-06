// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.Packets;
using Prion.Application.Timing;
using Prion.Application.Utilities;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Multiplayer.Client;
using Vitaru.Play;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;
using Vitaru.Server.Track;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class PlayTest : PlayRoot
    {
        private readonly Gamefield gamefield;
        private readonly SeekableClock seek;
        private readonly Track track;

        public PlayTest(SeekableClock seek, Track track)
        {
            this.seek = seek;
            this.track = track;

            gamefield = new Gamefield
            {
                Clock = seek
            };

            Player player = new Sakuya(gamefield)
            {
                Track = track
            };

            Add(player.InputHandler);

            gamefield.Add(player);

            //Packs
            Add(gamefield);

            //Layers
            Add(gamefield.ProjectileLayer);
            Add(gamefield.CharacterLayer);
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
        }

        public override void Update()
        {
            seek.NewFrame();
            track.CheckRepeat();
            if (track.CheckNewBeat())
            {
                for (int i = 0; i < gamefield.PlayerPack.Children.Count; i++)
                    gamefield.PlayerPack.Children[i].OnNewBeat();

                for (int i = 0; i < gamefield.LoadedEnemies.Children.Count; i++)
                    gamefield.LoadedEnemies.Children[i].OnNewBeat();
            }

            base.Update();
        }

        public override void PreRender()
        {
            base.PreRender();
            gamefield.PreRender();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.IsRepeat) return;

            switch (e.Key)
            {
                case Key.R:
                    enemy();
                    break;
            }
        }
    }
}