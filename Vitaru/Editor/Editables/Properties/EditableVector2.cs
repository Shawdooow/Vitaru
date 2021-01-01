// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableVector2 : EditableProperty<Vector2>
    {
        protected EditableVector2(IEditable e) : base(e)
        {
        }
    }
}