// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Server.Track
{
    [Serializable]
    public class LevelTrack
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

        /// <summary>
        ///     Song's Artist Name
        /// </summary>
        public string Artist;

        public double BPM;

        public double GetBeatLength() => 60000 / BPM;

        public double Offset;
    }
}