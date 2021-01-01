// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Vitaru.Server.Server;

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