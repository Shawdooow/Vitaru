#region usings

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class JoinMatchPacket : Packet
    {
        public override int PacketSize => 2048;

        public VitaruUser User;

        public MatchInfo Match;
    }
}
