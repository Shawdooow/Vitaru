// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Server.Track
{
    [Serializable]
    public class Level
    {
        /// <summary>
        ///     Information about the Song
        /// </summary>
        public LevelTrack LevelTrack;

        /// <summary>
        ///     The Version of vitaru this Level was made for (used to load old levels properly)
        /// </summary>
        public string Format;

        /// <summary>
        ///     Person who made this level, maybe this should be an array / have a seperate co-author field?
        /// </summary>
        public string LevelCreator;

        /// <summary>
        ///     What the Creator named this level, can be whatever
        /// </summary>
        public string LevelName;

        /// <summary>
        ///     The calculated difficulty of this level
        /// </summary>
        public double LevelDifficulty;

        /// <summary>
        ///     The gamemode this level was made for
        /// </summary>
        public string GamemodeName;
    }
}