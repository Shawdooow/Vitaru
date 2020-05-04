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
        private readonly Box shade;

        private readonly Gamefield gamefield;
        private readonly SeekableClock seek;
        private readonly Track track;

        private readonly VitaruServerNetHandler vitaruServer;
        private readonly VitaruNetHandler vitaruNet;

        public PlayTest(SeekableClock seek, Track track)
        {
            this.seek = seek;
            this.track = track;

            string address = "127.0.0.1:36840";
            //vitaruServer = new VitaruServerNetHandler
            //{
            //    Address = address
            //};
            //vitaruNet = new VitaruNetHandler
            //{
            //    Address = address,
            //    VitaruUser = new VitaruUser
            //    {
            //        Username = "Shawdooow",
            //        ID = 0,
            //    },
            //    OnPacketReceive = OnPacketRecieve
            //};

            gamefield = new Gamefield
            {
                Clock = seek
            };

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    shade = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    }
                }
            });

            Player player = new Sakuya(gamefield)
            {
                Track = track
            };

            Add(player.InputHandler);

            gamefield.Add(player);

            //Add(new Pack<Updatable>
            //{
            //    Children = new Updatable[]
            //    {
            //        vitaruServer,
            //        vitaruNet,
            //    }
            //});

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

            shade.FadeTo(0.8f, 1000);
            enemy();
            //vitaruNet.Connect();
            //createMatch();
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

        private void createMatch()
        {
            Level level = new Level
            {
                LevelTrack = track.Level,
                LevelCreator = "Shawdooow",
                LevelDifficulty = 2,
                LevelName = "Corona Man",
                GamemodeName = "Tau"
            };

            SendPacket(new CreateMatchPacket
            {
                MatchInfo = new MatchInfo
                {
                    Host = vitaruNet.VitaruUser,
                    Level = level
                }
            });
        }

        protected virtual void SendPacket(Packet packet) => vitaruNet.SendToServer(packet);

        protected virtual void OnPacketRecieve(PacketInfo<VitaruHost> info)
        {
            switch (info.Packet)
            {
                //Lobby Simulation
                case MatchListPacket matchListPacket:
                    //rooms.Children = new Container();
                    //foreach (MatchInfo m in matchListPacket.MatchInfoList)
                    //    rooms.Add(new MatchTile(vitaruNet, m));
                    break;
                case MatchCreatedPacket matchCreated:
                    //rooms.Add(new MatchTile(vitaruNet, matchCreated.MatchInfo));
                    SendPacket(new JoinMatchPacket
                    {
                        Match = matchCreated.MatchInfo,
                        User = vitaruNet.VitaruUser
                    });
                    break;
                case JoinedMatchPacket joinedMatch:
                    //Push(new MatchScreen(vitaruNet, joinedMatch));
                    break;
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Escape:
                    DropRoot();
                    break;
            }
        }
    }
}