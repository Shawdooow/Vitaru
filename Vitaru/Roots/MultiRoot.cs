// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.Packets;
using Prion.Game.Graphics.Roots;
using Vitaru.Multiplayer.Client;

namespace Vitaru.Roots
{
    public abstract class MultiRoot : Root
    {
        protected readonly VitaruNetHandler VitaruNet;

        protected MultiRoot(VitaruNetHandler vitaruNet)
        {
            VitaruNet = vitaruNet;
        }

        protected virtual void SendPacket(Packet packet) => VitaruNet.SendToServer(packet);

        protected virtual void OnPacketRecieve(PacketInfo<VitaruHost> info)
        {
        }
    }
}