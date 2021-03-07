using System;
using Prion.Centrosome.Packets.Types;

namespace Vitaru.Server.Packets.Lobby
{
    public class MatchListPacket : VariableLengthPacket
    {
        public MatchListPacket() : base(32)
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
