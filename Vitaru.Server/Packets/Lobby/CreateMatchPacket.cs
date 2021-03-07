using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Packets.Types;

namespace Vitaru.Server.Packets.Lobby
{
    public class CreateMatchPacket : MatchInfoPacket
    {
        public CreateMatchPacket() : base(31)
        {
        }

        public override IPacket Copy() => new CreateMatchPacket();
    }
}
