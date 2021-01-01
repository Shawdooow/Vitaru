﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableDouble : EditableProperty<double>
    {
        protected EditableDouble(IEditable e) : base(e)
        {
        }
    }
}