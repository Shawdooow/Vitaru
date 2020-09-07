using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitaru.Editor.Editables.Properties.Pattern
{
    public interface IHasPatternID : IEditable
    {
        short PatternID { get; set; }
    }
}
