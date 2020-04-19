#region usings

using System;
using Prion.Application.Networking.Packets;

#endregion

namespace Vitaru.Server.Packets.Play
{
    [Serializable]
    public class ScorePacket : Packet
    {
        public long ID;

        public int Score;
    }
}
