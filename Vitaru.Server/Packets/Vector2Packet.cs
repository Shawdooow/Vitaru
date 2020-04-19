// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using System.Numerics;
using System.Runtime.Serialization;

#endregion

namespace Vitaru.Server.Packets
{
    [Serializable]
    public class Vector2Packet : SharePacket
    {
        public float X => Vector2.X;
        public float Y => Vector2.Y;

        public virtual Vector2 Vector2 { get; set; } = Vector2.Zero;

        public Vector2Packet()
        {
        }

        public Vector2Packet(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Vector2 = new Vector2((float) info.GetValue("x", typeof(float)), (float) info.GetValue("y", typeof(float)));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("x", Vector2.X, typeof(float));
            info.AddValue("y", Vector2.Y, typeof(float));
        }
    }
}