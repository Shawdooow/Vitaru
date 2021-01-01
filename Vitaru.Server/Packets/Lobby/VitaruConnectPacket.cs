﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Centrosome.Packets.Connection;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    [Serializable]
    public class VitaruConnectPacket : ConnectPacket
    {
        public override int PacketSize => 1024;

        public VitaruUser User;
    }
}