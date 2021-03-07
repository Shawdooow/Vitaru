using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prion.Centrosome.Packets.Types;

namespace Vitaru.Server.Packets.Lobby
{
    public class JoinedMatchPacket : VariableLengthPacket
    {
        public JoinedMatchPacket() : base(37)
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
