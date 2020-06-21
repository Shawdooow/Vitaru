// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Centrosome.Packets;
using Vitaru.Server.Match;

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class JoinedMatchPacket : Packet
    {
        public override int PacketSize =>
            Convert.ToInt32(MatchInfo.Users.Count > 0 ? MatchInfo.Users.Count * 1024 + 1024 : 2048);

        public MatchInfo MatchInfo;
    }
}