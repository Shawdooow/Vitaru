// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.UserInterface;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.Packets;
using Vitaru.Multiplayer.Client;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;
using Vitaru.Server.Track;

namespace Vitaru.Roots.Multi
{
    public class MultiMenu : MenuRoot
    {
        private const string address = "127.0.0.1:36840";

        private readonly Pack<Updatable> networking;
        private readonly VitaruServerNetHandler vitaruServer;
        private readonly VitaruNetHandler vitaruNet;

        public MultiMenu()
        {
            Button create;
            Button join;

            Add(create = new Button
            {
                Position = new Vector2(-200, 0),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Host",

                OnClick = HostServer
            });
            Add(join = new Button
            {
                Position = new Vector2(200, 0),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Join",

                OnClick = () =>
                {
                    if (vitaruNet != null && vitaruNet.Host.Statues >= ConnectionStatues.Connected)
                        AddRoot(new Lobby(vitaruNet));
                    else
                        Logger.Error("Connect to a server first!", LogType.Network);
                }
            });

            create.BackgroundSprite.Color = Color.Orchid;
            join.BackgroundSprite.Color = Color.SpringGreen;

            Add(networking = new Pack<Updatable>());
        }

        protected virtual void JoinServer()
        {
            //if (OnlineModset.OsuNetworkingHandler != null)
            //{
            //    game.Remove(OnlineModset.OsuNetworkingHandler);
            //    OnlineModset.OsuNetworkingHandler.Dispose();
            //}
            //
            //try
            //{
            //    OnlineModset.OsuNetworkingHandler = new OsuNetworkingHandler
            //    {
            //        Address = ipBindable.Value + ":" + portBindable.Value,
            //    };
            //
            //    game.Add(OnlineModset.OsuNetworkingHandler);
            //    OnlineModset.OsuNetworkingHandler.OnConnectedToHost += host => Logger.Log("Connected to server", LoggingTarget.Network, LogLevel.Debug);
            //    OnlineModset.OsuNetworkingHandler.Connect();
            //}
            //catch (Exception e)
            //{
            //    Logger.Error(e, "Failed to create Networking Handler!", LoggingTarget.Network);
            //}
        }

        protected virtual void HostServer()
        {
            //if (OnlineModset.Server != null)
            //{
            //    OnlineModset.OsuNetworkingHandler.Remove(OnlineModset.Server);
            //    OnlineModset.Server.Dispose();
            //}
            //
            //if (OnlineModset.OsuNetworkingHandler != null)
            //{
            //    game.Remove(OnlineModset.OsuNetworkingHandler);
            //    OnlineModset.OsuNetworkingHandler.Dispose();
            //}
            //
            //try
            //{
            //    OnlineModset.Server = new OsuServerNetworkingHandler
            //    {
            //        Address = ipBindable.Value + ":" + portBindable.Value,
            //        //Udp = true,
            //    };
            //
            //    OnlineModset.OsuNetworkingHandler = new OsuNetworkingHandler
            //    {
            //        Address = ipBindable.Value + ":" + portBindable.Value,
            //    };
            //
            //    OnlineModset.OsuNetworkingHandler.Add(OnlineModset.Server);
            //
            //    game.Add(OnlineModset.OsuNetworkingHandler);
            //    OnlineModset.OsuNetworkingHandler.OnConnectedToHost += host => Logger.Log("Connected to local server", LogType.Network);
            //    OnlineModset.OsuNetworkingHandler.Connect();
            //}
            //catch (Exception e)
            //{
            //    Logger.Error(e, "Failed to create Networking Handler!");
            //}
        }

        private void createMatch()
        {
            Level level = new Level
            {
                //LevelTrack = track.Level,
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
    }
}