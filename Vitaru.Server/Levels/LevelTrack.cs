// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Server.Levels
{
    public class LevelTrack : ISerializableToBytes
    {
        /// <summary>
        ///     Name of the Song
        /// </summary>
        public string Title;

        /// <summary>
        ///     Name of the audio file
        /// </summary>
        public string Filename;

        /// <summary>
        ///     Name of the background image file
        /// </summary>
        public string Image = string.Empty;

        public bool Autoplay = true;

        /// <summary>
        ///     Song's Artist Name
        /// </summary>
        public string Artist;

        public double BPM;

        public double GetBeatLength() => 60000 / BPM;

        public double Offset;

        public double PreviewTime;

        public byte[] Serialize()
        {
            throw new NotImplementedException();

            List<byte> data = new();

            return data.ToArray();
        }

        public void DeSerialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}