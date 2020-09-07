using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
