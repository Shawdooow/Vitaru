using System;
using Vitaru.Editor.Editables;
using Vitaru.Server.Track;

namespace Vitaru.Editor
{
    public class LevelManager
    {
        public readonly Level Level;

        public EditableGenerator SelectedGenerator { get; private set; }

        public event Action<EditableGenerator> GeneratorSet;

        public IEditable SelectedEditable { get; private set; }

        public event Action<IEditable> EditableSet;

        public LevelManager(Level level)
        {
            Level = level;
        }

        public void SetGenerator(EditableGenerator generator)
        {
            SelectedGenerator = generator;
            GeneratorSet?.Invoke(generator);
        }
        
        public void SetEditable(IEditable editable)
        {
            SelectedEditable = editable;
            EditableSet?.Invoke(editable);
        }
    }
}
