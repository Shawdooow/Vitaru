// Copyright (c) 2018-2021 Shawn Bozek.
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

        private readonly Pack<Updatable> networking;

        protected MultiRoot(Pack<Updatable> networking)
        {
            this.networking = networking;
            VitaruNet = networking.Children[0] as VitaruNetHandler;
        }

        public override void LoadingComplete()
        {
            Add(networking);
            base.LoadingComplete();
        }

        protected override void OnPause()
        {
            Remove(networking, false);
            base.OnPause();
        }

        protected override void OnResume()
        {
            Add(networking);
            base.OnResume();
        }

        protected override void OnExiting()
        {
            Remove(networking, false);
            base.OnExiting();
        }

        protected virtual void SendPacketTcp(Packet packet) => VitaruNet.SendPacketTcp(packet);

        protected virtual void OnPacketRecieve(PacketInfo<VitaruHost> info)
        {
        }
    }
}