﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class CreateMatchPacket : Packet
    {
        public override int PacketSize => 2048;

        public MatchInfo MatchInfo;
    }
}