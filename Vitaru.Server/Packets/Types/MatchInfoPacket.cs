using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Prion.Centrosome;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Match;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Types
{
    public abstract class MatchInfoPacket : VariableLengthPacket
    {
        public MatchInfo MatchInfo;

        protected MatchInfoPacket(ushort header) : base(header)
        {
        }

        public override byte[] Serialize()
        {
            byte[] data = MatchInfo.Serialize();

            Length = (uint)data.Length;
            return data;
        }

        public override void DeSerialize(byte[] data)
        {
            MatchInfo = new MatchInfo();

            //string name = Encoding.ASCII.GetString();
        }
    }
}
