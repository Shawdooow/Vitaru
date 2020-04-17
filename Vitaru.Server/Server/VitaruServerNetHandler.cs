// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Net;
using System.Net.Sockets;
using Prion.Application.Networking.NetworkingHandlers.Server;

namespace Vitaru.Server.Server
{
    public class VitaruServerNetHandler : ServerNetHandler<VitaruServer, VitaruClient>
    {
        protected override string Gamekey => "vitaru";

        protected override VitaruServer GetClient() => new VitaruServer();

        protected override VitaruClient GetClient(TcpClient client, IPEndPoint end) =>
            new VitaruClient(PrionClient, client, end);
    }
}