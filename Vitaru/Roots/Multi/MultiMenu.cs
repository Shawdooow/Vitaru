// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.Packets;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Multiplayer.Client;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;
using Vitaru.Themes;

namespace Vitaru.Roots.Multi
{
    public class MultiMenu : MenuRoot
    {
        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private readonly TextBox ip;

        private readonly Pack<Updatable> networking;

        private VitaruServerNetHandler vitaruServer;
        private VitaruNetHandler vitaruNet;

        public MultiMenu()
        {
            Button host;
            Button connect;

            AddArray(new ILayer[]
            {
                ip = new TextBox
                {
                    Size = new Vector2(600, 60),
                    Position = new Vector2(0, -200),
                    Text = "127.0.0.1:36840"
                },
                host = new Button
                {
                    Position = new Vector2(-200, 0),
                    Size = new Vector2(200, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Text = "Host",

                    OnClick = HostServer
                },
                connect = new Button
                {
                    Position = new Vector2(200, 0),
                    Size = new Vector2(200, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Text = "Connect",

                    OnClick = JoinServer
                }
            });

            host.BackgroundSprite.Color = ThemeManager.SecondaryColor;
            connect.BackgroundSprite.Color = ThemeManager.PrimaryColor;

            Add(networking = new Pack<Updatable>());
        }

        protected virtual void JoinServer()
        {
            if (vitaruNet != null)
                networking.Remove(vitaruNet);

            try
            {
                networking.Add(vitaruNet = new VitaruNetHandler
                {
                    Address = ip.Text
                });

                vitaruNet.OnConnectedToHost += host =>
                {
                    Logger.Log("Connected to local server", LogType.Network);
                    AddRoot(new Lobby(vitaruNet));
                };
                vitaruNet.Connect();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create Networking Handler!", LogType.Network);
            }
        }

        protected virtual void HostServer()
        {
            if (vitaruServer != null)
                networking.Remove(vitaruServer);

            if (vitaruNet != null)
                networking.Remove(vitaruNet);

            try
            {
                networking.Add(vitaruServer = new VitaruServerNetHandler
                {
                    Address = ip.Text
                });

                networking.Add(vitaruNet = new VitaruNetHandler
                {
                    Address = ip.Text
                });

                vitaruNet.OnConnectedToHost += host =>
                {
                    Logger.Log("Connected to local server", LogType.Network);
                    AddRoot(new Lobby(vitaruNet));
                };
                vitaruNet.Connect();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create Local Networking Handler!");
            }
        }

        private void createMatch()
        {
            SendPacket(new CreateMatchPacket
            {
                MatchInfo = new MatchInfo
                {
                    Host = vitaruNet.VitaruUser
                    //Level = 
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