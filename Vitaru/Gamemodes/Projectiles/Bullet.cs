// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Numerics;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Utilities;
using Vitaru.Graphics.Particles;
using Vitaru.Settings;

namespace Vitaru.Gamemodes.Projectiles
{
    public class Bullet : Projectile
    {
        public static int COUNT;

        private readonly bool particles = global::Vitaru.Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private readonly float particles_multiplier =
            global::Vitaru.Vitaru.VitaruSettings.GetFloat(VitaruSetting.ParticleMultiplier);

        public override string Name { get; set; } = nameof(Bullet);

        public override DrawableGameEntity GenerateDrawable() => new DrawableBullet();

        public Vector2 EndPosition { get; protected set; }

        public float Distance { get; set; }

        public CurveType CurveType { get; set; }

        public int Curviness { get; set; }

        public float Speed { get; set; }

        public Easings SpeedEasing { get; set; }

        public float Diameter { get; set; } = 10;

        public Shape Shape { get; set; }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            UpdatePath();
            COUNT++;
        }

        protected virtual void UpdatePath()
        {
            EndPosition = StartPosition + PrionMath.Offset(Distance, Angle);

            switch (CurveType)
            {
                default:
                    EndTime = StartTime + Distance / Speed;
                    break;
                case CurveType.Target:
                    StartTime -= Distance / Speed / 2f;
                    break;
            }
        }

        private double s;

        public override void Update()
        {
            base.Update();
            Position = GetPosition(Clock.LastCurrent);

            s += Clock.LastElapsedTime;

            if (particles && PrionMath.RandomNumber(0, (int) s) >= 10 / particles_multiplier)
            {
                s = 0;
                OnAddParticle?.Invoke(new Particle(Position)
                {
                    Alpha = Drawable.Alpha,
                    Color = Color
                });
            }
        }

        protected virtual Vector2 GetPosition(double time)
        {
            double scale = Math.Clamp(PrionMath.Scale(time, StartTime, EndTime), 0, 1);
            return new Vector2(
                (float) PrionMath.Scale(Easing.ApplyEasing(SpeedEasing, scale), 0, 1, StartPosition.X,
                    EndPosition.X),
                (float) PrionMath.Scale(Easing.ApplyEasing(SpeedEasing, scale), 0, 1, StartPosition.Y,
                    EndPosition.Y));
        }

        public override void Start()
        {
            if (Drawable.LoadState < LoadState.Loaded) return;

            base.Start();

            Drawable.FadeTo(1, 200f, Easings.InSine);
            Drawable.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }

        public override void End()
        {
            if (ReturnedScore) return;
            base.End();

            ReturnGreat = false;
            ForceScore = true;

            Drawable.FadeTo(0, 250, Easings.InSine);
            Drawable.ScaleTo(new Vector2(1.5f), 250, Easings.OutCubic).OnComplete(UnLoad);
        }

        public override void ParseString(string[] data, int offset)
        {
            Diameter = float.Parse(data[0 + offset]);
            Speed = float.Parse(data[1 + offset]);

            base.ParseString(data, offset + 2);
        }

        public override string[] SerializeToStrings()
        {
            List<string> data = new List<string>
            {
                "b",
                Diameter.ToString(),
                Speed.ToString()
            };

            data.AddRange(base.SerializeToStrings());

            return data.ToArray();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            COUNT--;
        }
    }

    public enum CurveType
    {
        Straight,
        Target,

        CurveRight,
        CurveLeft
    }

    public enum Shape
    {
        Circle,
        Triangle,
        Square
    }
}