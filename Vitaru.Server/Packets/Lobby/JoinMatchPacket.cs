// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    public class JoinMatchPacket : VariableLengthPacket
    {
        public long Match;

        public VitaruUser User;

        public JoinMatchPacket() : base((ushort)VitaruPackets.JoinMatch)
        {
        }

        public override IPacket Copy()
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public override void DeSerialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}