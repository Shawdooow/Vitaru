// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Centrosome.NetworkingHandlers;
using Prion.Centrosome.Packets.Types;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Networking.Client;

namespace Vitaru.Roots.Multi
{
    public abstract class MultiRoot : MenuRoot
    {
        protected override bool UseLevelBackground => true;

        protected readonly VitaruNetHandler VitaruNet;

        protected readonly Pack<Updatable> Networking;

        protected MultiRoot(Pack<Updatable> networking)
        {
            Networking = networking;
            VitaruNet = networking.Children[0] as VitaruNetHandler;
        }

        public override void LoadingComplete()
        {
            Add(Networking);
            VitaruNet.OnPacketReceive = OnPacketRecieve;
            base.LoadingComplete();
        }

        protected override void OnPause()
        {
            Remove(Networking, false);
            base.OnPause();
        }

        protected override void OnResume()
        {
            Add(Networking);
            VitaruNet.OnPacketReceive = OnPacketRecieve;
            base.OnResume();
        }

        protected override void OnExiting()
        {
            Remove(Networking, false);
            base.OnExiting();
        }

        protected virtual void SendPacketTcp(Packet packet) => VitaruNet.SendPacketTcp(packet);

        protected virtual void OnPacketRecieve(PacketInfo<VitaruHost> info) { }
    }
}