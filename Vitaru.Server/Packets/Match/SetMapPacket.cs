// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using Vitaru.Server.Track;

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