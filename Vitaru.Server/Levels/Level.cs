// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Text;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;
using Prion.Ribosome.Audio;

namespace Vitaru.Server.Levels
{
    public class Level : ISerializableToBytes
    {
        /// <summary>
        ///     Metadata about the Song
        /// </summary>
        public TrackMetadata Metadata = new();

        /// <summary>
        ///     The Version of vitaru this Level was made for (used to load old levels properly)
        /// </summary>
        public string Format = "null";

        /// <summary>
        ///     Person who made this level, maybe this should be an array / have a seperate co-author field?
        /// </summary>
        public string Creator = "null";

        /// <summary>
        ///     What the Creator named this level, can be whatever
        /// </summary>
        public string Name = "null";

        /// <summary>
        ///     The calculated difficulty of this level
        /// </summary>
        public double Difficulty;

        /// <summary>
        ///     The gamemode this level was made for
        /// </summary>
        public string Gamemode = "null";

        /// <summary>
        ///     The serialized enemy data
        /// </summary>
        public string EnemyData = "null";

        public byte[] Serialize()
        {
            List<byte> data = new();

            //Fist lets convert each field one at a time to byte arrays
            byte[] metadata = Metadata.Serialize();
            byte[] format = Format.ToLengthAndBytes();
            byte[] creator = Creator.ToLengthAndBytes();
            byte[] name = Name.ToLengthAndBytes();
            byte[] difficulty = BitConverter.GetBytes(Difficulty);
            byte[] gamemode = Gamemode.ToLengthAndBytes();
            byte[] enemy = EnemyData.ToLengthAndBytes();

            //now lets add all the data to the master list
            data.AddRange(metadata);
            data.AddRange(format);
            data.AddRange(creator);
            data.AddRange(name);
            data.AddRange(difficulty);
            data.AddRange(gamemode);
            data.AddRange(enemy);

            //last we stick the size in.
            //while it is technically possible to deduce the size of the data on the other side it is wildly impractical to implement
            byte[] size = BitConverter.GetBytes(data.Count);
            data.InsertRange(0, size);

            return data.ToArray();
        }

        /// <summary>
        ///     Does NOT includes the 4 bytes of (int)size of this <see cref="Level" />
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            int offset = 0;

            //start with Metadata
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size = BitConverter.ToInt32(length);

            byte[] meta = data.SubArray(offset, size);
            offset += meta.Length;

            TrackMetadata m = new();
            m.DeSerialize(meta);
            Metadata = m;

            //Format
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] format = data.SubArray(offset, size);
            offset += format.Length;

            Format = Encoding.ASCII.GetString(format);

            //Creator
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] creator = data.SubArray(offset, size);
            offset += creator.Length;

            Creator = Encoding.ASCII.GetString(creator);

            //Name
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] name = data.SubArray(offset, size);
            offset += name.Length;

            Name = Encoding.ASCII.GetString(name);

            //Difficulty
            byte[] diff = data.SubArray(offset, 8);
            offset += diff.Length;

            Difficulty = BitConverter.ToDouble(diff);

            //Gamemode
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] gamemode = data.SubArray(offset, size);
            offset += gamemode.Length;

            Gamemode = Encoding.ASCII.GetString(gamemode);

            //Enemy
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] enemy = data.SubArray(offset, size);
            offset += enemy.Length;

            EnemyData = Encoding.ASCII.GetString(enemy);
        }
    }
}