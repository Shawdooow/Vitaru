using System.Numerics;

namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableVector2 : EditableProperty<Vector2>
    {
        protected EditableVector2(IEditable e) : base(e)
        {
        }
    }
}
