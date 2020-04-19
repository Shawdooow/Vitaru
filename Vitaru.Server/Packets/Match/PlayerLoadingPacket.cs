#region usings

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class PlayerLoadingPacket : Packet
    {
        public override int PacketSize => Convert.ToInt32(Match.Users.Count > 0 ? Match.Users.Count * 1024 : 1024);

        public MatchInfo Match;
    }
}
