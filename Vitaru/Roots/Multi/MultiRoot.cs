// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.Packets;
using Vitaru.Multiplayer.Client;

namespace Vitaru.Roots.Multi
{
    public abstract class MultiRoot : MenuRoot
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