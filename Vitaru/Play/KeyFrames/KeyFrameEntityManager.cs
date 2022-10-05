using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups;
using Prion.Nucleus.Groups.Packs;
using System;
using System.Collections.Generic;
using Vitaru.Editor.KeyFrames;

namespace Vitaru.Play.KeyFrames
{
    public abstract class KeyFrameEntityManager<T> : Pack<T>
        where T : IHasKeyFrames, IUpdatable
    {
        protected readonly List<T> UnloadedEntities = new List<T>();

        public virtual void Unload(T entity) => entity.PreLoaded = false;
        public virtual void Preload(T entity) => entity.PreLoaded = true;

        public virtual void Start(T entity) => entity.Started = true;
        public virtual void End(T entity) => entity.Started = false;

        public int[] Indexes =
        {
                0,
                0,
        };

        public bool MultiThreading { get; set; }

        public override void Update()
        {
            if (!MultiThreading) ProcessEntities(0, 1);
        }

        public override void Add(T child, AddPosition position = AddPosition.Last)
        {
            base.Add(child, position);
            if (!MultiThreading) Indexes[1] = ProtectedChildren.Count;
        }

        public override void Remove(T child, bool dispose = true)
        {
            base.Remove(child, dispose);
            if (!MultiThreading) Indexes[1] = ProtectedChildren.Count;
        }

        public void ProcessEntities(int s, int e)
        {
            Random r = new();
            int start = Indexes[s];
            int end = Indexes[e];

            for (int i = start; i < end; i++)
            {
                T t = ProtectedChildren[i];

                if (PlayManager.Current + t.TimePreLoad >= t.StartTime && PlayManager.Current < t.EndTime + t.TimeUnLoad && !t.PreLoaded)
                    Preload(t);
                else if ((PlayManager.Current + t.TimePreLoad < t.StartTime || PlayManager.Current >= t.EndTime + t.TimeUnLoad) &&
                         t.PreLoaded)
                    Unload(t);

                if (PlayManager.Current >= t.StartTime && PlayManager.Current < t.EndTime && !t.Started)
                    Start(t);
                else if ((PlayManager.Current < t.StartTime || PlayManager.Current >= t.EndTime) && t.Started)
                    End(t);

                //t.ConcurrentUpdate(r);
            }
        }
    }
}
