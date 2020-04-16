// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Entitys;
using Vitaru.Utilities;

namespace Vitaru.Projectiles
{
    public abstract class Projectile : Updatable, IHasTeam
    {
        public virtual int Team { get; set; }

        public float Angle { get; set; }

        public Vector2 StartPosition { get; set; }

        public double StartTime { get; set; }

        public float Damage { get; set; }

        public bool ObeyBoundries { get; set; }

        public bool TargetPlayer { get; set; }
    }
}