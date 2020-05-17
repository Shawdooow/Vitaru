// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Runtime.Serialization;

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class ValuePacket<T> : SharePacket
        where T : struct
    {
        public virtual T Value { get; set; }

        public ValuePacket()
        {
        }

        public ValuePacket(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Value = (T) info.GetValue("v", typeof(T));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("v", Value, typeof(T));
        }
    }
}