// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Editor.KeyFrames;
using Vitaru.Graphics.Projectiles.Bullets;

namespace Vitaru.Play.Projectiles
{
    public abstract class Projectile : GameEntity, IHasStartPosition, IHasKeyFrames
    {
        public override string Name { get; set; } = nameof(Projectile);

        protected BulletLayer BulletLayer { get; private set; }

        public new int Drawable { get; private set; } = -1;

        public void SetDrawable(int i, BulletLayer layer)
        {
            BulletLayer = layer;
            Drawable = i;
        }

        public virtual IDrawable2D GetOverlay(DrawableGameEntity draw)
        {
            throw Debugger.NotImplemented("");
        }

        public bool Selected { get; set; }

        public EditableProperty[] GetProperties() => new EditableProperty[]
        {
            new EditableStartPosition(this)
        };

        public List<KeyFrame> KeyFrames { get; set; } = new();

        public abstract Hitbox GetHitbox();

        public float Alpha = 0;

        public Color CircleColor = Color.White;

        public Color GlowColor = Color.Cyan;

        /// <summary>
        ///     Radians
        /// </summary>
        public float Angle { get; set; } = (float) Math.PI / -2f;

        public Vector2 StartPosition { get; set; }

        public virtual double StartTime { get; set; }

        public virtual double EndTime { get; set; }

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public float Damage { get; set; } = 20;

        public bool ObeyBoundries { get; set; } = true;

        public bool TargetPlayer { get; set; }

        public virtual bool Active => Started && !Collided;

        public bool Collided { get; protected set; }

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

        protected Random Random;

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Position = StartPosition;
        }

        public virtual void ConcurrentUpdate(Random random)
        {
            Random = random;
            Update();
        }

        public override void Update()
        {
            double current = Clock.Current;
            for (int i = 0; i < KeyFrames.Count; i++)
                KeyFrames[i].ApplyKeyFrame(current);

            if (Drawable != -1) UpdateDrawable();
        }

        /// <summary>
        ///     Called by Update Thread
        /// </summary>
        public virtual void UpdateDrawable()
        {
            BulletLayer.bPosition[Drawable] = Position;
            BulletLayer.bCircleColor[Drawable] = CircleColor.Vector(Alpha);
            BulletLayer.bGlowColor[Drawable] = GlowColor.Vector(Alpha);
        }

        public virtual void Collision()
        {
            Collided = true;
            //End();
        }

        public virtual void PreLoad() => PreLoaded = true;

        public virtual void Start() => Started = true;

        public virtual void End() => Started = false;

        public virtual void UnLoad() => PreLoaded = false;

        protected virtual double Weight(double distance)
        {
            return distance > 128 ? 0 : 500 / Math.Max(distance, 1);
        }

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

        protected override void Dispose(bool finalize)
        {
            if (Drawable != -1)
            {
                BulletLayer.ReturnIndex(Drawable);
                Drawable = -1;
            }

            BulletLayer = null;
            base.Dispose(finalize);
        }
    }
}