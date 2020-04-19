// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.NetworkingHandlers.Client;
using Prion.Application.Networking.Packets.Connection;
using Vitaru.Server.Server;

namespace Vitaru.Multiplayer.Client
{
    public sealed class VitaruNetHandler : ClientNetHandler<VitaruHost>
    {
        protected override string Gamekey => "vitaru";

        public VitaruUser VitaruUser { get; set; } = new VitaruUser();

        protected override VitaruHost GetClient() => new VitaruHost();

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
    }
}