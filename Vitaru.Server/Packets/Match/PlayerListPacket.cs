#region usings

using System;
using System.Collections.Generic;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class PlayerListPacket : Packet
    {
        public override int PacketSize => Convert.ToInt32(Players.Count > 0 ? Players.Count * 1024 + 1024 : 1024);

        public List<VitaruUser> Players = new List<VitaruUser>();
    }
}
