// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Entitys;
using Vitaru.Utilities;

namespace Vitaru.Projectiles
{
    public abstract class Projectile : Updatable, IHasTeam
    {
        public abstract DrawableProjectile GenerateDrawable();

        public virtual int Team { get; set; }

        public float Angle { get; set; }

        public Vector2 StartPosition { get; set; }

        public virtual double StartTime { get; set; }

        public virtual double EndTime { get; protected set; }

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public float Damage { get; set; }

        public bool ObeyBoundries { get; set; }

        public bool TargetPlayer { get; set; }

        public bool PreLoaded { get; private set; }

        public bool Started { get; private set; }

        public override void Update()
        {
            if (Clock.Current + TimePreLoad >= StartTime && Clock.Current < EndTime + TimeUnLoad && !PreLoaded)
                PreLoad();
            else if ((Clock.Current + TimePreLoad < StartTime || Clock.Current >= EndTime + TimeUnLoad) && PreLoaded)
                UnLoad();

            if (Clock.Current >= StartTime && Clock.Current < EndTime && !Started)
                Start();
            else if ((Clock.Current < StartTime || Clock.Current >= EndTime) && Started)
                End();
        }

        protected virtual void PreLoad() => PreLoaded = true;

        protected virtual void Start() => Started = true;

        protected virtual void End() => Started = false;

        protected virtual void UnLoad()
        {
            PreLoaded = false;
            //TODO: Dispose();
        }
    }
}