// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Text;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Server.Server
{
    public class Setting : ISerializableToBytes
    {
        public string Name = "null";

        public string Value = "null";

        public Sync Sync;

        public byte[] Serialize()
        {
            List<byte> data = new();

            byte[] name = Name.ToLengthAndBytes();
            byte[] value = Value.ToLengthAndBytes();
            byte[] sync = BitConverter.GetBytes((ushort)Sync);

            data.AddRange(name);
            data.AddRange(value);
            data.AddRange(sync);

            //last we stick the size in.
            //while it is technically possible to deduce the size of the data on the other side it is wildly impractical to implement
            byte[] size = BitConverter.GetBytes(data.Count);
            data.InsertRange(0, size);

            return data.ToArray();
        }

        /// <summary>
        ///     Does NOT includes the 4 bytes of (int)size of this <see cref="Setting" />
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            int offset = 0;

            //start with name
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size = BitConverter.ToInt32(length);

            byte[] name = data.SubArray(offset, size);
            offset += name.Length;

            Name = Encoding.ASCII.GetString(name);

            //Value
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] value = data.SubArray(offset, size);
            offset += value.Length;

            Value = Encoding.ASCII.GetString(value);

            //Sync
            byte[] sync = data.SubArray(offset, 2);
            offset += sync.Length;

            Sync = (Sync)BitConverter.ToUInt16(sync);
        }
    }

    public enum Sync
    {
        /// <summary>
        ///     This <see cref="Setting" /> does not need to be synced with the server or other clients.
        ///     EXAMPLE: Graphics quality
        /// </summary>
        None,

        /// <summary>
        ///     This <see cref="Setting" /> needs to be synced with the server but not necessarily with other clients until we load
        ///     in.
        ///     EXAMPLE: The character we want to play as
        /// </summary>
        Client,

        /// <summary>
        ///     This <see cref="Setting" /> needs to be the exact same across the server and all clients.
        ///     EXAMPLE: Increased enemy difficulty
        /// </summary>
        All,
    }
}