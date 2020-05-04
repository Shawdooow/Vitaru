﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Match;

#endregion

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class MatchCreatedPacket : Packet
    {
        public override int PacketSize => 1024;

        public MatchInfo MatchInfo;

        public bool Join;
    }
}