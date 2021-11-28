// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Match;

namespace Vitaru.Server.Packets.Types
{
    public abstract class MatchInfoPacket : VariableLengthPacket
    {
        public MatchInfo MatchInfo;

        protected MatchInfoPacket(ushort header) : base(header) { }

        public override byte[] Serialize()
        {
            byte[] data = MatchInfo.Serialize();

            Length = (uint)data.Length;
            return data;
        }

        public override void DeSerialize(byte[] data)
        {
            MatchInfo = new MatchInfo();
            MatchInfo.DeSerialize(data);
        }
    }
}