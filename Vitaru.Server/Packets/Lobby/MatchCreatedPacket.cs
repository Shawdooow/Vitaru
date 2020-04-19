#region usings

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class MatchCreatedPacket : Packet
    {
        public override int PacketSize => 1024;

        public MatchInfo MatchInfo;

        public bool Join;
    }
}
