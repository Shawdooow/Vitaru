using System;
using System.Collections.Generic;
using System.Text;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class CreateMatchPacket : Packet
    {
        public override int PacketSize => 2048;

        public MatchInfo MatchInfo;
    }
}
