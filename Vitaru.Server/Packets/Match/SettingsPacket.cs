#region usings

using System;
using Vitaru.Server.Server;

#endregion

namespace Vitaru.Server.Packets.Match
{
    [Serializable]
    public class SettingsPacket : OnlinePacket
    {
        public readonly Setting[] Settings;

        public SettingsPacket(Setting[] settings) => Settings = settings;

        public SettingsPacket(Setting setting) => Settings = new[] {setting};
    }
}
