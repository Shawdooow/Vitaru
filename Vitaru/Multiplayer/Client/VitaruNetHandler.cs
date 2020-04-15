// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Networking.NetworkingHandlers.Client;

namespace Vitaru.Multiplayer.Client
{
    public sealed class VitaruNetHandler : ClientNetHandler<VitaruHost>
    {
        protected override string Gamekey => "vitaru";

        protected override VitaruHost GetClient() => new VitaruHost();
    }
}