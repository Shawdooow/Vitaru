// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

namespace Vitaru.Editor.Editables.Properties.Position
{
    public class EditableStartPosition : EditableVector2
    {
        protected readonly IHasStartPosition Entity;

        public EditableStartPosition(IHasStartPosition e) : base(e)
        {
            Entity = e;
        }

        public override Vector2 Value => Entity.StartPosition;

        public override void SetValue(Vector2 t)
        {
            Entity.StartPosition = t;
            Entity.Position = t;
            base.SetValue(t);
        }
    }
}