// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.Packets;
using Prion.Application.Utilities;
using Prion.Game.Audio;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Vitaru.Editor;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters;
using Vitaru.Multiplayer.Client;
using Vitaru.Play;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;

namespace Vitaru.Roots.Tests
{
    public class PlayTest : PlayRoot
    {
        private readonly Gamefield gamefield;

        private readonly VitaruServerNetHandler vitaruServer;
        private readonly VitaruNetHandler vitaruNet;

        private readonly SeekableClock seek;
        private readonly RepeatableSample track;

        public PlayTest(SeekableClock seek, RepeatableSample track)
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
                    new Box
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

            enemy();
            //vitaruNet.Connect();
            //createMatch();
        }

        public override void Update()
        {
            seek.NewFrame();
            track.CheckRepeat();
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
                LevelTrack = new LevelTrack
                {
                    Name = "Alki Bells",
                    Artist = "Shawdooow"
                },
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