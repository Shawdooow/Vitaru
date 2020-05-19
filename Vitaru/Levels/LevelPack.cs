// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Core.Utilities.Interfaces;
using Vitaru.Server.Track;

namespace Vitaru.Levels
{
    public class LevelPack : IHasName
    {
        public string Name { get; internal set; }

        public Level[] Levels;
    }
}