// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prion.Centrosome;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Server.Server
{
    public class Setting : ISerializableToBytes
    {
        public string Name;

        public string Value;

        public Sync Sync;

        public byte[] Serialize()
        {
            List<byte> data = new();

            byte[] name = Name.ToLengthAndBytes();
            byte[] value = Value.ToLengthAndBytes();
            byte[] sync = Unsafe.As<Sync, byte[]>(ref Sync);

            data.AddRange(name);
            data.AddRange(value);
            data.AddRange(sync);

            return data.ToArray();
        }

        public void DeSerialize(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum Sync
    {
        None,
        Client,
        All
    }
}