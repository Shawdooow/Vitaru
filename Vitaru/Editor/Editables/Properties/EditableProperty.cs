// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableProperty<T> : EditableProperty
        where T : struct
    {
        public Action<T> ValueUpdated;

        public abstract T Value { get; set; }

        protected EditableProperty(IEditable e) : base(e)
        {
        }
    }

    public abstract class EditableProperty
    {
        protected EditableProperty(IEditable e)
        {
        }
    }
}