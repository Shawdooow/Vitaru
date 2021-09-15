// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.Packets.Types;
using Prion.Nucleus.Debug;

namespace Vitaru.Server.Packets.Lobby
{
    public class JoinedMatchPacket : VariableLengthPacket
    {
        public JoinedMatchPacket() : base((ushort)VitaruPackets.JoinedMatch) { }

        public override IPacket Copy() => throw Debugger.NotImplemented("");

        public override byte[] Serialize() => throw Debugger.NotImplemented("");

        public override void DeSerialize(byte[] data)
        {
            throw Debugger.NotImplemented("");
        }
    }
}