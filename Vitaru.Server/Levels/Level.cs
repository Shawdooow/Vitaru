// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Nucleus.Utilities.Interfaces;
using Prion.Ribosome.Audio;

namespace Vitaru.Server.Levels
{
    public class Level : ISerializableToBytes
    {
        /// <summary>
        ///     Metadata about the Song
        /// </summary>
        public TrackMetadata Metadata;

        /// <summary>
        ///     The Version of vitaru this Level was made for (used to load old levels properly)
        /// </summary>
        public string Format;

        /// <summary>
        ///     Person who made this level, maybe this should be an array / have a seperate co-author field?
        /// </summary>
        public string Creator;

        /// <summary>
        ///     What the Creator named this level, can be whatever
        /// </summary>
        public string Name;

        /// <summary>
        ///     The calculated difficulty of this level
        /// </summary>
        public double LevelDifficulty;

        /// <summary>
        ///     The gamemode this level was made for
        /// </summary>
        public string GamemodeName;

        /// <summary>
        ///     The serialized enemy data
        /// </summary>
        public string EnemyData;

        public byte[] Serialize()
        {
            throw new NotImplementedException();

#pragma warning disable 162
            List<byte> data = new();

            return data.ToArray();
#pragma warning restore 162
        }

        /// <summary>
        /// Does NOT includes the 4 bytes of (int)size of this <see cref="Setting"/>
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}