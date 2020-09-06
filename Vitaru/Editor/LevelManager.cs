using System;
using Vitaru.Editor.Editables;
using Vitaru.Server.Track;

namespace Vitaru.Editor
{
    public class LevelManager
    {
        public readonly Level Level;

        public Action<Editable> EditableSelected;

        public LevelManager(Level level)
        {
            Level = level;
        }
    }
}
