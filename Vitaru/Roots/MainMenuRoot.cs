// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Application.Entitys;
using Prion.Application.Groups.Packs;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.Packets;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Audio;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Characters.Enemies;
using Vitaru.Characters.Players;
using Vitaru.Multiplayer.Client;
using Vitaru.Play;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;

namespace Vitaru.Roots
{
    public class MainMenuRoot : Root
    {
        private readonly Gamefield gamefield;

        private readonly VitaruServerNetHandler vitaruServer;
        private readonly VitaruNetHandler vitaruNet;

        private AudioDevice device;

        public MainMenuRoot()
        {
            device = new AudioDevice();

            Sample sample = new Sample("alki main theme menu bells.mp3");

            sample.Play();

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

            gamefield = new Gamefield();

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("Backgrounds\\medicine.png"))
                    {
                        Scale = new Vector2(2f),
                    },
                    new Sprite(Game.TextureStore.GetTexture("Backgrounds\\vitaru spring 2018.png"))
                    {
                        Scale = new Vector2(0.75f),
                    },
                    new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5),
                    },
                },
            });

            Player player = new Player(gamefield);

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
            Add(gamefield.PlayerPack);
            Add(gamefield.LoadedEnemies);
            Add(gamefield.ProjectilePack);

            //Layers
            Add(gamefield.ProjectileLayer);
            Add(gamefield.CharacterLayer);
        }

        private void enemy()
        {
            gamefield.Add(new Enemy(gamefield)
            {
                StartTime = Clock.Current,
                StartPosition = new Vector2(PrionMath.RandomNumber(-200, 200), PrionMath.RandomNumber(-300, 0)),
                OnDie = enemy,
            });
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            enemy();
            //vitaruNet.Connect();
            //createMatch();
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
                LevelTitle = "Debug Level",
                LevelArtist = "Shawdooow",
                LevelCreator = "Shawdooow",
                LevelDifficulty = 2,
                LevelName = "Corona Man",
                GamemodeName = "Tau",
            };

            SendPacket(new CreateMatchPacket
            {
                MatchInfo = new MatchInfo
                {
                    Host = vitaruNet.VitaruUser,
                    Level = level,
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
                        User = vitaruNet.VitaruUser,
                    });
                    break;
                case JoinedMatchPacket joinedMatch:
                    //Push(new MatchScreen(vitaruNet, joinedMatch));
                    break;
            }
        }
    }
}