﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.NetworkingHandlers.Client;
using Vitaru.Server.Server;

namespace Vitaru.Networking.Client
{
    public sealed class VitaruNetHandler : ClientNetHandler<VitaruHost>
    {
        public VitaruUser VitaruUser { get; set; } = new();

        protected override VitaruHost GetClient() => new();
    }
}