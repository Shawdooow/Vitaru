﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes
{
    public abstract class Chapter
    {
        public abstract string Title { get; }

        public virtual string Description => null;

        public abstract Player[] GetPlayers(Gamefield gamefield = null);
    }
}