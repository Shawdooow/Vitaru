// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Server.Server;
using Vitaru.Server.Track;

namespace Vitaru.Play
{
    public class LevelData : Level
    {
        public List<Enemy> Enemies { get; set; }
    }
}