// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Editor.IO;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Characters.Enemies
{
    public class Enemy : Character, IEditable
    {
        public const int ENEMY_TEAM = 0;

        public override float HitboxDiameter => 48f;

        public virtual DrawableEnemy GenerateDrawable()
        {
            DrawableEnemy draw = new DrawableEnemy(this);
            Drawable = draw;
            return draw;
        }

        public Vector2 StartPosition { get; set; }

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
            base.Update();

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

        protected virtual void UnLoad() => PreLoaded = false;

        protected override void Die()
        {
            base.Die();
            EndTime = Clock.Current;
            Drawable.Delete();
            Gamefield.Remove(this);
        }

        public virtual void ParseString(string[] data)
        {
            StartTime = double.Parse(data[0]);
            EndTime = double.Parse(data[1]);
            StartPosition = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
            HitboxDiameter = float.Parse(data[4]);
        }

        public virtual string[] SerializeToStrings()
        {
            return new[]
            {
                StartTime.ToString(),
                EndTime.ToString(),
                $"{StartPosition.X},{StartPosition.Y}",
                HitboxDiameter.ToString(),
            };
        }
    }
}