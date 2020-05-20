// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Server.Track
{
    [Serializable]
    public class LevelTrack
    {
        public string Name;

        public string Filename;

        public string Image = string.Empty;

        public string Artist;

        public double BPM;

        public double GetBeatLength() => 60000 / BPM;

        public double Offset;
    }
}