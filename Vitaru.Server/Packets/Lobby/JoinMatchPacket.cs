using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    public class JoinMatchPacket : VariableLengthPacket
    {
        public long Match;

        public VitaruUser User;

        public JoinMatchPacket() : base(36)
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
