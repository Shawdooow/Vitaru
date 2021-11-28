// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Editor.Editables.Properties.Time
{
    public class EditableEndTime : EditableDouble
    {
        protected readonly IHasEndTime Entity;

        public EditableEndTime(IHasEndTime e) : base(e)
        {
            Entity = e;
        }

        public override double Value => Entity.EndTime;

        public override void SetValue(double t)
        {
            Entity.EndTime = t;
            base.SetValue(t);
        }
    }
}