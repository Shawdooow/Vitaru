using System;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Match;

namespace Vitaru.Server.Packets.Lobby
{
    public class MatchCreatedPacket : VariableLengthPacket
    {
        public MatchInfo Match;

        public MatchCreatedPacket() : base(34)
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
