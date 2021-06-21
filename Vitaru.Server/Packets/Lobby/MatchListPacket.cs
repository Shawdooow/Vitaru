// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Match;

namespace Vitaru.Server.Packets.Lobby
{
    public class MatchListPacket : VariableLengthPacket
    {
        public List<MatchInfo> MatchInfos;

        public MatchListPacket() : base((ushort)VitaruPackets.MatchList)
        {
        }

        public override IPacket Copy() => new MatchListPacket();

        public override byte[] Serialize()
        {
            List<byte> data = new();

            int length = MatchInfos.Count;

            data.AddRange(BitConverter.GetBytes(length));

            foreach (MatchInfo match in MatchInfos)
                data.AddRange(match.Serialize());

            return data.ToArray();
        }

        public override void DeSerialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}