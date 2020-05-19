using Prion.Core.Utilities.Interfaces;
using Vitaru.Server.Track;

namespace Vitaru.Levels
{
    public class LevelPack : IHasName
    {
        public string Name { get; internal set; }

        public Level[] Levels;
    }
}
