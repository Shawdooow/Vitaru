﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes
{
    public abstract class Chapter
    {
        public abstract string Title { get; }

        public virtual string Description => null;

        public virtual Vector2 PlayfieldAspectRatio => new Vector2(5, 4);

        public virtual Vector2 PlayfieldSize => new Vector2(1024, 820);

        public virtual Vector4 PlayfieldBounds => new Vector4(0, 0, PlayfieldSize.X, PlayfieldSize.Y);

        public virtual Vector2 PlayerStartingPosition => new Vector2(PlayfieldSize.X / 2, 700);

        public virtual Vector2 ClusterOffset => new Vector2(256, 0);

        public abstract Player[] GetPlayers(Gamefield gamefield = null);
    }
}