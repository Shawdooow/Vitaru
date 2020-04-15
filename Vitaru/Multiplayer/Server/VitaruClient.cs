// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Net;
using System.Net.Sockets;

namespace Vitaru.Multiplayer.Server
{
    public class VitaruClient : Prion.Application.Networking.NetworkingHandlers.Server.Client
    {
        public VitaruClient(Prion.Application.Networking.NetworkingHandlers.Server.Server server, TcpClient client,
            IPEndPoint end) : base(server, client, end)
        {
        }
    }
}