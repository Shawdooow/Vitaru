#region usings

using System;
using System.Runtime.Serialization;

#endregion

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
            Value = (T)info.GetValue("v", typeof(T));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("v", Value, typeof(T));
        }
    }
}
