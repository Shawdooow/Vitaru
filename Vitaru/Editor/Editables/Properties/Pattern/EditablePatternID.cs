// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Editor.Editables.Properties.Pattern
{
    public class EditablePatternID : EditableShort
    {
        protected readonly IHasPatternID Entity;

        public EditablePatternID(IHasPatternID e) : base(e)
        {
            Entity = e;
        }

        public override short Value => Entity.PatternID;

        public override void SetValue(short t)
        {
            Entity.PatternID = t;
            base.SetValue(t);
        }
    }
}