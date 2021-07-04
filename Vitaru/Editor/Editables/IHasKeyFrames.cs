using System.Collections.Generic;

namespace Vitaru.Editor.Editables
{
    public interface IHasKeyFrames : IEditable
    {
        List<KeyFrame> KeyFrames { get; set; }
    }
}
