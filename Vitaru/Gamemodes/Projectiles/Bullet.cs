// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Numerics;
using Prion.Application.Debug;
using Prion.Application.Utilities;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Transforms;

namespace Vitaru.Gamemodes.Projectiles
{
    public class Bullet : Projectile
    {
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
        }

        protected virtual void UpdatePath()
        {
            Vector2 offset = new Vector2(Distance * MathF.Cos(Angle), Distance * MathF.Sin(Angle));
            EndPosition = StartPosition + offset;

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

        public override void Update()
        {
            base.Update();
            Position = GetPosition(Clock.LastCurrent);
        }

        protected virtual Vector2 GetPosition(double time)
        {
            return new Vector2(
                (float) PrionMath.Scale(Easing.ApplyEasing(SpeedEasing, time), StartTime, EndTime, StartPosition.X,
                    EndPosition.X),
                (float) PrionMath.Scale(Easing.ApplyEasing(SpeedEasing, time), StartTime, EndTime, StartPosition.Y,
                    EndPosition.Y));
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