using Prion.Nucleus.Debug;
using Prion.Nucleus.Groups;
using Prion.Nucleus.Groups.Packs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vitaru.Editor.KeyFrames;
using Vitaru.Play.Characters.Enemies;

namespace Vitaru.Play.KeyFrames
{
    public abstract class KeyFrameEntityManager<T> : Pack<T>
        where T : GameEntity, IHasKeyFrames
    {
        protected readonly List<T> UnloadedEntities = new List<T>();
        protected readonly ConcurrentQueue<T> UnloadQueue = new ConcurrentQueue<T>();

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
            double current = Clock.Current;
            //Lets check our unloaded entities to see if any need to be drawn soon, if so lets load them
            for (int i = 0; i < UnloadedEntities.Count; i++)
            {
                T c = UnloadedEntities[i];
                if (current >= c.StartTime - c.TimePreLoad && current < c.EndTime + c.TimeUnLoad)
                {
                    UnloadedEntities.Remove(c);
                    Add(c, AddPosition.Last);
                }
            }

            if (!MultiThreading) ProcessEntities(0, 1);

            //should be safe to kill them from here
            while (UnloadQueue.TryDequeue(out T c))
            {
                Debugger.Assert(!c.Disposed, $"Disposed {nameof(Enemy)}s shouldn't be in the {nameof(UnloadQueue)}!");
                Remove(c, false);
                UnloadedEntities.Add(c);
            }
        }

        public virtual void Add(T child)
        {
            UnloadedEntities.Add(child);
            //child.OnAddParticle = ParticleLayer.Add;
        }

        public virtual void Remove(T child)
        {
            Debugger.Assert(!child.Disposed,
                $"Disposed {nameof(Enemy)}s shouldn't be getting added to {nameof(UnloadQueue)}!");
            Debugger.Assert(!UnloadQueue.Contains(child),
                $"{nameof(Enemy)} shouldn't be getting added to {nameof(UnloadQueue)} again!");
            //que them since we may be calling this from outside the main update thread
            UnloadQueue.Enqueue(child);
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
