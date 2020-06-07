// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Nucleus.Networking.Packets;

namespace Vitaru.Server.Packets.Play
{
    [Serializable]
    public class NewEnemyPacket : Packet
    {
        public override int PacketSize => 2048;

        public Vector2 Position;
    }
}