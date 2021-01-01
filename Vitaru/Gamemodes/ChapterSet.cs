// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

namespace Vitaru.Gamemodes
{
    //Old gamemode class
    public abstract class Chapterset
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Chapterset.";

        public abstract Chapter[] GetChapters();

        public virtual Vector2 PlayfieldSize => new(1024, 820);

        public virtual Vector4 PlayfieldBounds => new(0, 0, PlayfieldSize.X, PlayfieldSize.Y);

        public virtual Vector2 PlayerStartingPosition => new(PlayfieldSize.X / 2, 700);
    }
}