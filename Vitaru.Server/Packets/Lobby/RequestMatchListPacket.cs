using Prion.Centrosome.Packets.Types;

namespace Vitaru.Server.Packets.Lobby
{
    public class RequestMatchListPacket : BlankPacket
    {
        public override IPacket Copy() => new RequestMatchListPacket();

        public RequestMatchListPacket() : base((ushort)VitaruPackets.RequestMatchList)
        {

        }
    }
}
