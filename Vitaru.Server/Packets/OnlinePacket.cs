// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Centrosome.Packets;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class OnlinePacket : Packet
    {
        public override int PacketSize => 2048;

        public VitaruUser User;
    }
}