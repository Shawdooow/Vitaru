// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Server.Track;

namespace Vitaru.Levels
{
    public class LevelPack
    {
        /// <summary>
        ///     Name of the Song
        /// </summary>
        public string Title { get; internal set; }

        public Level[] Levels;
    }
}