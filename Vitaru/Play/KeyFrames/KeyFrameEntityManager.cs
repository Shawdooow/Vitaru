using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using System.Collections.Generic;
using Vitaru.Editor.KeyFrames;

namespace Vitaru.Play.KeyFrames
{
    public abstract class KeyFrameEntityManager<T> : Pack<T>
        where T : IHasKeyFrames, IUpdatable
    {
        protected readonly List<T> UnloadedEntities = new List<T>();


    }
}
