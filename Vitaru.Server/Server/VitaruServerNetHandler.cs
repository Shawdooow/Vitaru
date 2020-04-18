// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Prion.Application.Debug;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.NetworkingHandlers.Server;

namespace Vitaru.Server.Server
{
    public class VitaruServerNetHandler : ServerNetHandler<VitaruServer, VitaruClient>
    {
        protected override string Gamekey => "vitaru";

        protected readonly List<VitaruMatch> OsuMatches = new List<VitaruMatch>();

        protected override VitaruServer GetClient() => new VitaruServer();

        protected override VitaruClient GetClient(TcpClient client, IPEndPoint end) =>
            new VitaruClient(PrionClient, client, end)
            {
                LastConnection = Clock.Current,
                Statues = ConnectionStatues.Connecting,
            };

        protected class VitaruMatch
        {
            public MatchInfo MatchInfo;

            public List<VitaruClient> Clients = new List<VitaruClient>();

            public List<VitaruClient> LoadedClients = new List<VitaruClient>();

            public double MatchLastUpdateTime;

            public bool Add(VitaruClient client)
            {
                if (Clients.Contains(client) || LoadedClients.Contains(client) || MatchInfo.Users.Contains(client.User))
                {
                    Logger.Log($"({client.User.Username} - {client.TcpEndPoint}) tried to be added to a match they already in!?", LogType.Network);
                    return false;
                }

                Clients.Add(client);
                MatchInfo.Users.Add(client.User);

                return true;
            }

            public bool Remove(VitaruClient client)
            {
                if ((Clients.Contains(client) || LoadedClients.Contains(client)) && MatchInfo.Users.Contains(client.User))
                {
                    Clients.Remove(client);
                    MatchInfo.Users.Remove(client.User);

                    return true;
                }

                Logger.Error($"({client.User.Username} - {client.TcpEndPoint}) tried to be removed from a match they aren't in!?", LogType.Network);
                return false;
            }

            public bool Loaded(VitaruClient client)
            {
                if (Clients.Contains(client) && !LoadedClients.Contains(client))
                {
                    Clients.Remove(client);
                    LoadedClients.Add(client);

                    return true;
                }

                Logger.Error($"({client.User.Username} - {client.TcpEndPoint}) is already loaded?", LogType.Network);
                return false;
            }

            public bool UnLoaded(VitaruClient client)
            {
                if (!Clients.Contains(client) && LoadedClients.Contains(client))
                {
                    LoadedClients.Remove(client);
                    Clients.Add(client);

                    return true;
                }

                Logger.Error($"({client.User.Username} - {client.TcpEndPoint}) is already unloaded?", LogType.Network);
                return false;
            }
        }
    }
}