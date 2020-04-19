#region usings

using System;

#endregion

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class ChatPacket : OnlinePacket
    {
        public string AuthorColor;

        public string Message;
    }
}
