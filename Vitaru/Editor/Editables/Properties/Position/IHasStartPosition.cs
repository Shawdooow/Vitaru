using System.Numerics;

namespace Vitaru.Editor.Editables.Properties.Position
{
    public interface IHasStartPosition : IEditable
    {
        Vector2 StartPosition { get; set; }
    }
}
