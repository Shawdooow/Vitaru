﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableProperty<T> : EditableProperty
        where T : struct
    {
        public event Action<T> OnValueUpdated;

        public abstract T Value { get; }

        public virtual void SetValue(T t) => OnValueUpdated?.Invoke(t);

        protected EditableProperty(IEditable e) : base(e) { }
    }

    public abstract class EditableProperty
    {
        protected EditableProperty(IEditable e) { }
    }
}