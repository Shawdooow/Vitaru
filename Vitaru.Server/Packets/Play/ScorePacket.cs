// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Centrosome.Packets;

namespace Vitaru.Server.Packets.Play
{
    [Serializable]
    public class ScorePacket : Packet
    {
        public long ID;

        public int Score;
    }
}