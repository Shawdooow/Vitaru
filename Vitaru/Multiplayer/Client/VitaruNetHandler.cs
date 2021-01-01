﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.NetworkingHandlers.Client;
using Prion.Centrosome.Packets;
using Prion.Centrosome.Packets.Connection;
using Vitaru.Server.Packets;
using Vitaru.Server.Packets.Lobby;
using Vitaru.Server.Server;

namespace Vitaru.Multiplayer.Client
{
    public sealed class VitaruNetHandler : ClientNetHandler<VitaruHost>
    {
        protected override string Gamekey => "vitaru";

        public VitaruUser VitaruUser { get; set; } = new();

        protected override VitaruHost GetClient() => new();

        protected override void PacketReceived(PacketInfo<VitaruHost> info)
        {
            base.PacketReceived(info);

            switch (info.Packet)
            {
                case TestPacket _:
                    Ping();
                    break;
            }
        }

        public override void Connect()
        {
            base.Connect();
            SendToServer(new VitaruConnectPacket());
        }

        protected override Packet SignPacket(Packet packet)
        {
            switch (packet)
            {
                case VitaruConnectPacket connectPacket:
                    connectPacket.User = VitaruUser;
                    break;
                case OnlinePacket onlinePacket:
                    onlinePacket.User = VitaruUser;
                    break;
            }

            return base.SignPacket(packet);
        }
    }
}