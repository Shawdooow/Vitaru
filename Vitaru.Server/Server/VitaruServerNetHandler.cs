// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Prion.Application.Debug;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.NetworkingHandlers.Server;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Match;
using Vitaru.Server.Packets;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Packets.Match;
using Vitaru.Server.Packets.Play;

namespace Vitaru.Server.Server
{
    public class VitaruServerNetHandler : ServerNetHandler<VitaruServer, VitaruClient>
    {
        protected override string Gamekey => "vitaru";

        protected readonly List<VitaruMatch> VitaruMatches = new List<VitaruMatch>();

        protected override VitaruServer GetClient() => new VitaruServer();

        protected uint MatchID;

        protected override VitaruClient GetClient(TcpClient client, IPEndPoint end) =>
            new VitaruClient(PrionClient, client, end)
            {
                LastConnection = Clock.Current,
                Statues = ConnectionStatues.Connecting
            };

        protected override void PacketReceived(PacketInfo<VitaruClient> info)
        {
            base.PacketReceived(info);

            VitaruMatch match;

            switch (info.Packet)
            {
                default:
                    base.PacketReceived(info);
                    break;

                case VitaruConnectPacket connectPacket:
                    base.PacketReceived(info);
                    info.Client.User = connectPacket.User;
                    break;

                #region Score

                //case ScoreSubmissionPacket scoreSubmission:
                //    OnlineScoreStore.SetScore(scoreSubmission);
                //    break;

                #endregion

                #region Import

                //case ImportPacket import:
                //    if (Maps == null)
                //    {
                //        List<string> paths = new List<string>();
                //        foreach (string path in Stable.GetDirectories($"Songs"))
                //        {
                //            string modified = "";
                //            for (int i = 6; i < path.Length; i++)
                //                modified = modified + path[i];
                //            paths.Add(modified);
                //        }
                //
                //        Maps = paths.ToArray();
                //    }
                //
                //    QueImport(info.Client);
                //    break;
                //case SendMapPacket send:
                //    if (sending) break;
                //    if (ImportingClients.ContainsKey(info.Client))
                //    {
                //        ImportingClients[info.Client]++;
                //        SendClientImportMap(Maps[ImportingClients[info.Client]], info.Client);
                //    }
                //
                //    break;

                #endregion

                #region Multi

                #region Lobby

                case GetMatchListPacket getMatch:
                    //Send them a list of matches
                    MatchListPacket matchList = new MatchListPacket
                    {
                        MatchInfoList = GetMatches()
                    };
                    SendToPeer(matchList, info.Client);
                    break;
                case CreateMatchPacket createMatch:

                    //A bit hacky, but makes searching easier and more accurate
                    createMatch.MatchInfo.MatchID = MatchID;
                    MatchID++;

                    //Add the new match
                    VitaruMatches.Add(new VitaruMatch
                    {
                        MatchInfo = createMatch.MatchInfo,
                        MatchLastUpdateTime = Clock.Current
                    });
                    SendToPeer(new MatchCreatedPacket
                    {
                        MatchInfo = createMatch.MatchInfo,
                        //Make them join this match since they made it!
                        Join = true
                    }, info.Client);
                    break;
                case JoinMatchPacket joinPacket:
                    foreach (VitaruMatch m in VitaruMatches)
                        if (m.MatchInfo.MatchID == joinPacket.Match.MatchID)
                        {
                            match = m;

                            //Add them
                            VitaruClient Vitaru = FindClient(joinPacket.User);
                            Vitaru.User = joinPacket.User;
                            if (!match.Add(Vitaru)) break;

                            Vitaru.OnDisconnected += () => match.Remove(Vitaru);

                            //Tell everyone already there someone joined
                            ShareWithMatchClients(match.MatchInfo, new PlayerJoinedPacket
                            {
                                User = joinPacket.User
                            });

                            //Tell them they have joined
                            SendToPeer(new JoinedMatchPacket {MatchInfo = match.MatchInfo}, info.Client);
                            break;
                        }

                    Logger.Error("Couldn't find an VitaruMatch matching one in packet!");
                    break;

                #endregion

                #region Match

                case GetMapPacket getMap:
                    match = FindMatch(getMap.User);

                    //Tell them what map the Match is set to
                    SendToPeer(new SetMapPacket(match.MatchInfo.Level), info.Client);
                    break;
                case SetMapPacket level:
                    match = FindMatch(level.User);

                    //Set our map
                    match.MatchInfo.Level = level.Level;

                    //Tell everyone that a new map was set
                    ShareWithMatchClients(match, level);
                    break;
                case SettingsPacket setting:
                    match = FindMatch(setting.User);

                    //Hacky was of setting the Setting but should work
                    if (setting.Settings[0].Sync == Sync.All)
                    {
                        for (int i = 0; i < match.MatchInfo.Settings.Count; i++)
                            if (match.MatchInfo.Settings[i].Name == setting.Settings[0].Name)
                            {
                                match.MatchInfo.Settings[i] = setting.Settings[0];
                                goto finish;
                            }

                        //Hacky way of adding ones we dont have but should work for now
                        match.MatchInfo.Settings.Add(setting.Settings[0]);

                        finish:
                        //Send them ALL the settings just incase, you never know with these things...
                        ShareWithMatchClients(match, new SettingsPacket(match.MatchInfo.Settings.ToArray()));
                    }
                    else if (setting.Settings[0].Sync == Sync.Client)
                    {
                        VitaruUser user = FindClient(setting.User).User;
                        for (int i = 0; i < user.UserSettings.Count; i++)
                            if (user.UserSettings[i].Name == setting.Settings[0].Name)
                            {
                                user.UserSettings[i] = setting.Settings[0];
                                goto end;
                            }

                        //Hacky way of adding ones we dont have but should work for now
                        user.UserSettings.Add(setting.Settings[0]);
                    }

                    end:
                    break;
                case StatuesChangePacket statuesChange:
                    match = FindMatch(statuesChange.User);

                    //Set their statues
                    FindClient(statuesChange.User).User.Statues = statuesChange.User.Statues;

                    //Tell everyone they changed their statues
                    ShareWithMatchClients(match, statuesChange);
                    break;
                case ChatPacket chat:
                    //Nothing we need to do on our end currently, just fire it right back out
                    ShareWithMatchClients(FindMatch(chat.User), chat);
                    break;
                case LeavePacket leave:
                    match = FindMatch(leave.User);
                    if (match.Remove(FindClient(leave.User)))
                    {
                        //Tell everyone someone rage quit
                        ShareWithMatchClients(match, new PlayerDisconnectedPacket
                        {
                            User = leave.User
                        });

                        //Update their matchlist next
                        MatchListPacket list = new MatchListPacket();
                        list = (MatchListPacket) SignPacket(list);
                        list.MatchInfoList = GetMatches();

                        TcpNetworkingClient.SendPacket(list, TcpNetworkingClient.EndPoint);
                    }

                    break;
                case LoadPlayerPacket load:
                    match = FindMatch(load.User);
                    ShareWithMatchClients(match, new PlayerLoadingPacket
                    {
                        Match = match.MatchInfo
                    });
                    break;

                #endregion

                #region Play

                case PlayerLoadedPacket loaded:
                    VitaruClient c = FindClient(loaded.User);
                    match = FindMatch(loaded.User);

                    match.Clients.Remove(c);
                    match.LoadedClients.Add(c);

                    if (match.Clients.Count == 0)
                        ShareWithMatchClients(match.MatchInfo, new MatchStartingPacket());
                    break;
                case ScorePacket score:
                    match = FindMatch(FindClient(score.ID).User);
                    ShareWithMatchClients(match.MatchInfo, score);
                    break;
                case SharePacket share:
                    match = FindMatch(FindClient(share.ID).User);
                    ShareWithMatchClients(match.MatchInfo, share);
                    break;
                case MatchExitPacket exit:
                    match = FindMatch(exit.User);

                    restart:
                    foreach (VitaruClient r in match.LoadedClients)
                    {
                        match.LoadedClients.Remove(r);
                        match.Clients.Add(r);
                        goto restart;
                    }

                    ShareWithMatchClients(match.MatchInfo, exit);

                    break;

                #endregion

                #endregion
            }
        }

        public override void Update()
        {
            base.Update();

            restart:
            foreach (VitaruMatch match in VitaruMatches)
            {
                if (match.MatchInfo.Users.Count == 0 && match.MatchLastUpdateTime + 60000 <= Clock.Current)
                {
                    VitaruMatches.Remove(match);
                    Logger.Log("Empty match deleted!");
                    goto restart;
                }

                if (match.MatchInfo.Users.Count > 0)
                {
                    match.MatchLastUpdateTime = Clock.Current;
                }
            }
        }

        protected void ShareWithMatchClients(VitaruMatch match, Packet packet) =>
            ShareWithMatchClients(match.MatchInfo, packet);

        protected void ShareWithMatchClients(MatchInfo match, Packet packet)
        {
            foreach (VitaruUser user in match.Users)
                TcpNetworkingClient.SendPacket(packet, FindClient(user).TcpEndPoint);
        }

        /// <summary>
        ///     Exists since VitaruMatch isn't serializable
        /// </summary>
        /// <returns></returns>
        protected List<MatchInfo> GetMatches()
        {
            List<MatchInfo> matches = new List<MatchInfo>();

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

            public List<VitaruClient> Clients = new List<VitaruClient>();

            public List<VitaruClient> LoadedClients = new List<VitaruClient>();

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