// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Editor.Editables.Properties.Color
{
    public class EditableColor : EditableProperty<System.Drawing.Color>
    {
        protected readonly IHasColor Entity;

        public EditableColor(IHasColor e) : base(e)
        {
            Entity = e;
        }

        public override System.Drawing.Color Value => Entity.Color;

        public override void SetValue(System.Drawing.Color t)
        {
            Entity.Color = t;
            base.SetValue(t);
        }
    }
}