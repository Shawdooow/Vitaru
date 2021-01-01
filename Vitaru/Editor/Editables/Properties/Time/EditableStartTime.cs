// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Editor.Editables.Properties.Time
{
    public class EditableStartTime : EditableDouble
    {
        protected readonly IHasStartTime Entity;

        public EditableStartTime(IHasStartTime e) : base(e)
        {
            Entity = e;
        }

        public override double Value => Entity.StartTime;

        public override void SetValue(double t)
        {
            Entity.StartTime = t;
            base.SetValue(t);
        }
    }
}