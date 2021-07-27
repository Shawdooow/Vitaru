// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Graphics.Particles;
using Vitaru.Play;
using Vitaru.Settings;

namespace Vitaru.Gamemodes.Projectiles
{
    public class Bullet : Projectile
    {
        public static int COUNT;

        private readonly bool particles = global::Vitaru.Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private readonly float particles_multiplier =
            global::Vitaru.Vitaru.VitaruSettings.GetFloat(VitaruSetting.ParticleMultiplier);

        private readonly Vector2 border = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize() / 2;

        public override string Name { get; set; } = nameof(Bullet);

        public override DrawableGameEntity GenerateDrawable() => null;

        public Vector2 EndPosition { get; protected set; }

        public float Distance { get; set; }

        public CurveType CurveType { get; set; }

        public float CurveAmount { get; set; }

        protected Vector2 CurvePoint { get; set; }

        protected List<Vector2> CurvePoints { get; set; } = new();

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
            EndTime = StartTime + Distance / Speed;

            switch (CurveType)
            {
                case CurveType.Target:
                    StartTime -= Distance / Speed / 2f;
                    break;
                case CurveType.Bezier:
                    //Half way to the EndPoint
                    CurvePoint = StartPosition + PrionMath.Offset(Distance / 2, Angle) + 
                                 //Then add the "CurveAmount" to slide it to one side or the other
                                 PrionMath.Offset(CurveAmount, Angle - MathF.PI / -2f);
                    break;
            }
        }

        private double s;

        public override void Update()
        {
            Position = GetPosition(Gamefield.Current);

            if (Position.X < -border.X || Position.X > border.X || Position.Y < -border.Y || Position.Y > border.Y)
                End();

            s += Gamefield.LastElapsedTime;

            if (particles && s >= 10 / particles_multiplier && Started && CurveType < CurveType.Target)
            {
                s = 0;

                float angle = ((float) Random.Next(0, 360)).ToRadians();
                int distance = Random.Next(20, 32);

                int radius = (int) Diameter / 2 - 8;

                Vector2 start = Position + new Vector2(Random.Next(-radius, radius),
                    Random.Next(-radius, radius));

                OnAddParticle?.Invoke(new Particle
                {
                    StartPosition = start,
                    EndPosition = start + PrionMath.Offset(distance, angle),
                    Color = Color.Vector(),
                    Scale = 1f / Random.Next(1, 5)
                });
            }

            //UpdateDrawable Last
            base.Update();
        }

        public override void UpdateDrawable()
        {
            base.UpdateDrawable();
            BulletLayer.bSize[Drawable] = new Vector2(Diameter);
        }

        protected virtual Vector2 GetPosition(double time)
        {
            double scale = Math.Clamp(PrionMath.Remap(time, StartTime, EndTime), 0, 1);
            float t = (float)Easing.ApplyEasing(SpeedEasing, scale);

            switch (CurveType)
            {
                default:
                    return new Vector2( PrionMath.Remap(t, 0, 1, StartPosition.X, EndPosition.X),
                        PrionMath.Remap(t, 0, 1, StartPosition.Y, EndPosition.Y));
                case CurveType.Bezier:
                    return bezier(t, StartPosition, CurvePoint, EndPosition);
            }
        }

        private Vector2 bezier(float t, Vector2 start, Vector2 mid, Vector2 end)
        {
            Vector2 ab = Vector2.Lerp(start, mid, t);
            Vector2 bc = Vector2.Lerp(mid, end, t);
            return Vector2.Lerp(ab, bc, t);
        }

        public override void Start()
        {
            base.Start();

            Alpha = 1;
            //Drawable.FadeTo(1, 200f, Easings.InSine);
            //Drawable.ScaleTo(Vector2.One, 100f, Easings.InSine);
        }

        public override void End()
        {
            if (ReturnedScore) return;
            base.End();

            ReturnGreat = false;
            ForceScore = true;

            //Drawable.FadeTo(0, 250, Easings.InSine);
            //Drawable.ScaleTo(new Vector2(1.5f), 250, Easings.OutCubic).OnComplete(UnLoad);
            Alpha = 0;
        }

        public override void ParseString(string[] data, int offset)
        {
            Diameter = float.Parse(data[0 + offset]);
            Speed = float.Parse(data[1 + offset]);

            base.ParseString(data, offset + 2);
        }

        public override string[] SerializeToStrings()
        {
            List<string> data = new()
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

        Bezier,
    }

    public enum Shape
    {
        Circle,
        Triangle,
        Square
    }
}