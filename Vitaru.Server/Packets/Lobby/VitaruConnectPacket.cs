#region usings

using System;
using Prion.Application.Networking.Packets.Connection;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class VitaruConnectPacket : ConnectPacket
    {
        public override int PacketSize => 1024;

        public VitaruUser User;
    }
}
