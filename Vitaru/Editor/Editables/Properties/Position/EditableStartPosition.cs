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

        public override Vector2 Value
        {
            get => Entity.StartPosition;
            set => Entity.StartPosition = value;
        }
    }
}
