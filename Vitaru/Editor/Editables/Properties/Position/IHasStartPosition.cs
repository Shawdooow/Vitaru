// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

namespace Vitaru.Editor.Editables.Properties.Position
{
    public interface IHasStartPosition : IEditable
    {
        Vector2 StartPosition { get; set; }
    }
}