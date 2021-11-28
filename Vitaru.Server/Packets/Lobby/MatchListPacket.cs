// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Centrosome.Packets.Types;
using Prion.Nucleus.Debug;
using Vitaru.Server.Match;

namespace Vitaru.Server.Packets.Lobby
{
    public class MatchListPacket : VariableLengthPacket
    {
        public List<MatchInfo> MatchInfos = new();

        public MatchListPacket() : base((ushort)VitaruPackets.MatchList) { }

        public override IPacket Copy() => new MatchListPacket();

        public override byte[] Serialize()
        {
            List<byte> data = new();

            int length = MatchInfos.Count;

            if (length == 0) return data.ToArray();

            data.AddRange(BitConverter.GetBytes(length));

            foreach (MatchInfo match in MatchInfos)
                data.AddRange(match.Serialize());

            return data.ToArray();
        }

        public override void DeSerialize(byte[] data)
        {
            throw Debugger.NotImplemented("");
        }
    }
}