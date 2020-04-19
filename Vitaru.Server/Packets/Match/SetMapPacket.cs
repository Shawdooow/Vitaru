#region usings

using System;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class SetMapPacket : OnlinePacket
    {
        public readonly Level Level;

        public SetMapPacket(Level level) => Level = level;
    }
}
