// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Runtime.Serialization;

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class ValueArrayPacket<T, Y> : SharePacket
        where T : struct
    {
        public override int PacketSize => 8192;

        public virtual T Value { get; set; }

        public virtual Y[] Array { get; set; }

        public ValueArrayPacket()
        {
        }

        public ValueArrayPacket(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Value = (T) info.GetValue("v", typeof(T));
            Array = (Y[]) info.GetValue("a", typeof(Y[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("v", Value, typeof(T));
            info.AddValue("a", Array, typeof(Y[]));
        }
    }
}