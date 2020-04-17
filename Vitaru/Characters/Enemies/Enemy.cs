﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;

namespace Vitaru.Characters.Enemies
{
    public class Enemy : Character
    {
        public const int ENEMY_TEAM = 0;

        public virtual DrawableEnemy GenerateDrawable()
        {
            DrawableEnemy draw = new DrawableEnemy(this)
            {
                Y = -200f,
            };
            Drawable = draw;
            return draw;
        }

        public virtual double StartTime { get; set; }

        public virtual double EndTime { get; protected set; } = double.MaxValue;

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public bool PreLoaded { get; private set; }

        public bool Started { get; private set; }

        public Enemy(Gamefield gamefield) : base(gamefield)
        {
            Team = ENEMY_TEAM;
        }

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

            if (Drawable is null) return;

            Drawable.Position = new Vector2(200 * MathF.Sin((float) Clock.Current / 500f), Drawable.Y);
        }

        protected virtual void PreLoad() => PreLoaded = true;

        protected virtual void Start() => Started = true;

        protected virtual void End() => Started = false;

        protected virtual void UnLoad()
        {
            PreLoaded = false;
            Dispose();
        }
    }
}