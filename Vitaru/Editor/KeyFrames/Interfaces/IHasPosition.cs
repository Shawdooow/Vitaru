// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

namespace Vitaru.Editor.KeyFrames.Interfaces
{
    public interface IHasPosition
    {
        public Vector2 Position { get; set; }
    }
}
