// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Graphics.Particles;

namespace Vitaru.Gamemodes.Projectiles
{
    public abstract class Projectile : GameEntity, IHasStartPosition
    {
        public override string Name { get; set; } = nameof(Projectile);

        public Action<Particle> OnAddParticle;

        public override void SetDrawable(DrawableGameEntity drawable)
        {
            DrawableProjectile draw = drawable as DrawableProjectile;
            draw.SetProjectile(this);
            base.SetDrawable(drawable);
        }

        public EditableProperty[] GetProperties() => new EditableProperty[]
        {
            new EditableStartPosition(this),
        };

        public Color Color = Color.White;

        public float Angle { get; set; } = (float) Math.PI / -2f;

        public Vector2 StartPosition { get; set; }

        public virtual double StartTime { get; set; }

        public virtual double EndTime { get; protected set; }

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public float Damage { get; set; } = 20;

        public bool ObeyBoundries { get; set; }

        public bool TargetPlayer { get; set; }

        public bool PreLoaded { get; private set; }

        public bool Started { get; private set; }

        //Can be set for the Graze ScoringMetric
        public double MinDistance = double.MaxValue;

        //Set to "true" when a score should be returned
        public bool ForceScore;

        //Set to "true" when a score has been returned
        protected bool ReturnedScore;

        //return a great
        public bool ReturnGreat;

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Position = StartPosition;
        }

        public override void Update()
        {
        }

        public virtual void Hit() => End();

        public virtual void PreLoad() => PreLoaded = true;

        public virtual void Start() => Started = true;

        public virtual void End() => Started = false;

        public event Action OnUnLoad;

        public virtual void UnLoad()
        {
            PreLoaded = false;
            OnUnLoad?.Invoke();
        }

        protected virtual double Weight(double distance)
        {
            return distance > 128 ? 0 : 500 / Math.Max(distance, 1);
        }

        public virtual void Delete() => Drawable.Delete();

        public virtual void ParseString(string[] data, int offset)
        {
            StartTime = double.Parse(data[0 + offset]);
            EndTime = double.Parse(data[1 + offset]);
            StartPosition = new Vector2(float.Parse(data[2 + offset]), float.Parse(data[3 + offset]));
            Damage = float.Parse(data[4 + offset]);
        }

        public virtual string[] SerializeToStrings()
        {
            return new[]
            {
                StartTime.ToString(),
                EndTime.ToString(),
                StartPosition.ToString(),
                Damage.ToString()
            };
        }
    }
}