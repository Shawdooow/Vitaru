// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Chapters
{
    public abstract class Chapter
    {
        public abstract string Title { get; }

        public virtual string Description => null;

        public virtual Vector2 PlayfieldAspectRatio => new(5, 4);

        public virtual Vector2 PlayfieldSize => new(1024, 820);

        public virtual Vector4 PlayfieldBounds => new(0, 0, PlayfieldSize.X, PlayfieldSize.Y);

        public virtual Vector2 PlayerStartingPosition => new(PlayfieldSize.X / 2, 700);

        public virtual Vector2 ClusterOffset => new(256, 0);

        public abstract Player[] GetPlayers(Gamefield gamefield = null);
    }
}