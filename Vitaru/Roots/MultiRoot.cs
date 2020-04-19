using System;
using System.Collections.Generic;
using System.Text;
using Prion.Application.Networking.NetworkingHandlers;
using Prion.Application.Networking.NetworkingHandlers.Client;
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
