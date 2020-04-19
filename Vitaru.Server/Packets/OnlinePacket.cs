#region usings

using System;
using Prion.Application.Networking.Packets;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class OnlinePacket : Packet
    {
        public override int PacketSize => 2048;

        public VitaruUser User;
    }
}
