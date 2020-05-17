// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Runtime.Serialization;
using Prion.Core.Networking.Packets;

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class SharePacket : Packet, ISerializable
    {
        public override int PacketSize => 1024;

        public virtual string Name { get; set; }

        public virtual long ID { get; set; }

        public SharePacket()
        {
        }

        public SharePacket(SerializationInfo info, StreamingContext context)
        {
            ID = (long) info.GetValue("i", typeof(long));
            Name = info.GetString("n");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("i", ID);
            info.AddValue("n", Name);
        }
    }
}