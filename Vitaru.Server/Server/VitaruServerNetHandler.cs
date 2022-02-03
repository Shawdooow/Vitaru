// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.NetworkingHandlers.Server;
using Prion.Centrosome.Packets;
using Prion.Centrosome.Packets.Types;
using Prion.Nucleus.Debug;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;

namespace Vitaru.Server.Server
{
    public class VitaruServerNetHandler : ServerNetHandler<VitaruServer, VitaruClient>
    {
        static VitaruServerNetHandler()
        {
            PacketManager.RegisterPacket(new RequestMatchListPacket());
            PacketManager.RegisterPacket(new MatchListPacket());
            PacketManager.RegisterPacket(new CreateMatchPacket());
            PacketManager.RegisterPacket(new MatchCreatedPacket());
            PacketManager.RegisterPacket(new JoinMatchPacket());
            PacketManager.RegisterPacket(new JoinedMatchPacket());
        }

        protected readonly List<VitaruMatch> VitaruMatches = new();

        protected override VitaruServer GetClient() => new();

        protected override VitaruClient GetClient(TcpClient client, IPEndPoint end) =>
            new(PrionClient, client, end)
            {
                LastConnection = Clock.Current,
                Status = ConnectionStatus.Connecting,
            };

        public override void Update()
        {
            base.Update();

            foreach (VitaruMatch match in VitaruMatches)
            {
                if (match.MatchInfo.Users.Count == 0 && match.MatchLastUpdateTime + 60000 <= Clock.Current)
                {
                    VitaruMatches.Remove(match);
                    Logger.Log("Empty match expired and has been deleted!");
                }

                if (match.MatchInfo.Users.Count > 0)
                {
                    match.MatchLastUpdateTime = Clock.Current;
                }
            }
        }

        protected override void PacketReceived(PacketInfo<VitaruClient> info)
        {
            base.PacketReceived(info);

            switch (info.Packet)
            {
                case RequestMatchListPacket:
                    SendToClient(new MatchListPacket(), info.Client);
                    break;
                case CreateMatchPacket create:
                    VitaruMatch match = new()
                    {
                        MatchInfo = create.MatchInfo,
                        MatchLastUpdateTime = Clock.Current,
                    };
                    match.MatchInfo.ID = VitaruMatches.Any() ? VitaruMatches.Last().MatchInfo.ID + 1 : 1;
                    VitaruMatches.Add(match);

                    SendToClient(new MatchCreatedPacket
                    {
                        MatchInfo = match.MatchInfo,
                    }, info.Client);
                    break;
                case JoinMatchPacket join:
                    match = FindMatch(join.Match);
                    if (match.Add(FindClient(join.User.ID)))
                    {
                        SendToClient(new JoinedMatchPacket
                        {

                        }, info.Client);
                    }
                    else
                        Logger.Warning("Player failed to join a match!", LogType.Network);
                    break;
            }
        }

        protected void ShareWithMatchClients(VitaruMatch match, Packet packet) =>
            ShareWithMatchClients(match.MatchInfo, packet);

        protected void ShareWithMatchClients(MatchInfo match, Packet packet)
        {
            foreach (VitaruUser user in match.Users)
                SendPacketTcp(packet, FindClient(user).TcpEndPoint);
        }

        /// <summary>
        ///     Exists since VitaruMatch isn't serializable
        /// </summary>
        /// <returns></returns>
        protected List<MatchInfo> GetMatches()
        {
            List<MatchInfo> matches = new();

            foreach (VitaruMatch match in VitaruMatches)
                matches.Add(match.MatchInfo);

            return matches;
        }

        protected VitaruMatch FindMatch(VitaruUser player)
        {
            foreach (VitaruMatch m in VitaruMatches)
                foreach (VitaruUser p in m.MatchInfo.Users)
                    if (p.ID == player.ID)
                        return m;
            return null;
        }

        protected VitaruMatch FindMatch(uint id)
        {
            foreach (VitaruMatch m in VitaruMatches)
                if (m.MatchInfo.ID == id)
                    return m;
            return null;
        }

        /// <summary>
        ///     Exists since VitaruClient isn't serializable
        /// </summary>
        /// <returns></returns>
        protected VitaruClient FindClient(VitaruUser user) => FindClient(user.ID);

        protected VitaruClient FindClient(long id)
        {
            foreach (VitaruClient p in Clients)
                if (p.User.ID == id)
                    return p;
            return null;
        }

        protected class VitaruMatch
        {
            public MatchInfo MatchInfo;

            public List<VitaruClient> Clients = new();

            public List<VitaruClient> LoadedClients = new();

            public double MatchLastUpdateTime;

            public bool Add(VitaruClient client)
            {
                if (Clients.Contains(client) || LoadedClients.Contains(client) || MatchInfo.Users.Contains(client.User))
                {
                    Logger.Log(
                        $"({client.User.Username} - {client.TcpEndPoint}) tried to be added to a match they already in!?",
                        LogType.Network);
                    return false;
                }

                Clients.Add(client);
                MatchInfo.Users.Add(client.User);

                return true;
            }

            public bool Remove(VitaruClient client)
            {
                if ((Clients.Contains(client) || LoadedClients.Contains(client)) &&
                    MatchInfo.Users.Contains(client.User))
                {
                    Clients.Remove(client);
                    MatchInfo.Users.Remove(client.User);

                    return true;
                }

                Logger.Error(
                    $"({client.User.Username} - {client.TcpEndPoint}) tried to be removed from a match they aren't in!?",
                    LogType.Network);
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