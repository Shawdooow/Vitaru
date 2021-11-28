// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Play;

namespace Vitaru.Editor.Editables
{
    public abstract class EditableGenerator
    {
        public abstract IEditable GetEditable(Gamefield field);
    }
}