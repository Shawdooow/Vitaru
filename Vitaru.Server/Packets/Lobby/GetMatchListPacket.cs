#region usings

using System;
using Prion.Application.Networking.Packets;

#endregion

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class GetMatchListPacket : Packet
    {
        public override int PacketSize => 256;
    }
}
