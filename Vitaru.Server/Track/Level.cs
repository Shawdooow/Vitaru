// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Server.Track
{
    [Serializable]
    public class Level
    {
        public LevelTrack LevelTrack;

        public string LevelCreator;

        public string LevelName;

        public double LevelDifficulty;

        public string GamemodeName;
    }
}