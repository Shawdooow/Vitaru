// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Centrosome.Packets;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class PlayerListPacket : Packet
    {
        public override int PacketSize => Convert.ToInt32(Players.Count > 0 ? Players.Count * 1024 + 1024 : 1024);

        public List<VitaruUser> Players = new();
    }
}