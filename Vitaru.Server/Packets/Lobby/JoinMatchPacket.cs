// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Centrosome.Packets.Types;
using Prion.Nucleus.Utilities;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Lobby
{
    public class JoinMatchPacket : VariableLengthPacket
    {
        public uint Match;

        public VitaruUser User;

        public JoinMatchPacket() : base((ushort)VitaruPackets.JoinMatch) { }

        public override IPacket Copy() => new JoinMatchPacket();

        public override byte[] Serialize()
        {
            List<byte> data = new();

            byte[] match = BitConverter.GetBytes(Match);
            byte[] user = User.Serialize();

            data.AddRange(match);
            data.AddRange(user);

            //last we stick the size in.
            //while it is technically possible to deduce the size of the data on the other side it is wildly impractical to implement
            byte[] size = BitConverter.GetBytes(data.Count);
            data.InsertRange(0, size);

            return data.ToArray();
        }

        public override void DeSerialize(byte[] data)
        {
            int offset = 0;

            //start with match
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size;

            byte[] match = data.SubArray(offset, 4);
            offset += match.Length;

            Match = BitConverter.ToUInt32(match);

            //now do the user
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] user = data.SubArray(offset, size);
            offset += user.Length;

            VitaruUser u = new();
            u.DeSerialize(user);
            User = u;
        }
    }
}