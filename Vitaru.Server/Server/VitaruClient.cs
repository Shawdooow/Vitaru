// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Net;
using System.Net.Sockets;
using Prion.Nucleus.Networking.NetworkingHandlers.Server;

namespace Vitaru.Server.Server
{
    public class VitaruClient : Client
    {
        public VitaruUser User;

        public VitaruClient(Prion.Nucleus.Networking.NetworkingHandlers.Server.Server server, TcpClient client,
            IPEndPoint end) : base(server, client, end)
        {
        }
    }
}