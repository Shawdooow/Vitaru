// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Packets.Types;

namespace Vitaru.Server.Packets.Lobby
{
    public class MatchCreatedPacket : MatchInfoPacket
    {
        public MatchCreatedPacket() : base((ushort)VitaruPackets.MatchCreated)
        {
        }

        public override IPacket Copy() => new MatchCreatedPacket();
    }
}