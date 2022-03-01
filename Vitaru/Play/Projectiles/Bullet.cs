// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames.Interfaces;
using Vitaru.Gamemodes;
using Vitaru.Graphics.Particles;
using Vitaru.Settings;
using Vitaru.Editor.KeyFrames;

namespace Vitaru.Play.Projectiles
{
    public class Bullet : Projectile, IHasAlpha, IHasScale, IHasPosition
    {
        public static int COUNT;

        private readonly bool particles = Vitaru.VitaruSettings.GetBool(VitaruSetting.Particles);

        private readonly float particles_multiplier =
            Vitaru.VitaruSettings.GetFloat(VitaruSetting.ParticleMultiplier);

        private readonly Vector2 border = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize() / 2;

        public override string Name { get; set; } = nameof(Bullet);

        public override DrawableGameEntity GenerateDrawable() => null;

        public override Hitbox GetHitbox() => CircularHitbox;

        public CircularHitbox CircularHitbox = new()
        {
            Diameter = 10,
        };

        public Vector2 Size
        {
            get => new(CircularHitbox.Diameter);
            set => CircularHitbox.Diameter = value.X;
        }

        public Vector2 Scale 
        { 
            get => scale;
            set
            {
                scale = value;
                if (Drawable != -1)
                    BulletLayer.bSize[Drawable] = Size.X * Scale.X;
            }
        }

        private Vector2 scale = Vector2.One;

        public override float Alpha
        {
            get => alpha;
            set
            {
                alpha = Math.Clamp(value, 0, 1);
                if (Drawable != -1)
                    BulletLayer.bCircleColor[Drawable].W = alpha;
            }
        }

        private float alpha;

        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                CircularHitbox.Position = value;
            }
        }

        public Vector2 EndPosition { get; protected set; }

        public float Distance { get; set; }

        public CurveType CurveType { get; set; }

        public float CurveAmount { get; set; }

        protected Vector2 CurvePoint { get; set; }

        public float Speed { get; set; }

        public Easings SpeedEasing { get; set; }

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
                                 PrionMath.Offset(CurveAmount, Angle - MathF.PI / 2f);
                    break;
            }
        }

        private double s;

        public override void Update()
        {
            if (!Vitaru.EnableKeyFrames)
                Position = GetPosition(Gamefield.Current);

            if (Vitaru.EnableKeyFrames)
                for (int i = 0; i < KeyFrames.Count; i++)
                    KeyFrame.ApplyFrames(Gamefield.Current, KeyFrames[i].Value);

            if (ObeyBoundries && (Position.X < -border.X || Position.X > border.X || Position.Y < -border.Y ||
                                  Position.Y > border.Y))
                End();

            s += Gamefield.LastElapsedTime;

            //Emit Particles?
            if (particles && s >= 10 / particles_multiplier && Started)
            {
                s = 0;

                float angle = ((float)Random.Next(0, 360)).ToRadians();
                int distance = Random.Next(20, 32);

                int radius = (int)CircularHitbox.Radius - 8;

                Vector2 start = Position + new Vector2(Random.Next(-radius, radius),
                    Random.Next(-radius, radius));

                OnAddParticle?.Invoke(new Particle
                {
                    StartPosition = start,
                    EndPosition = start + PrionMath.Offset(distance, angle),
                    Color = GlowColor.Vector(),
                    Scale = 1f / Random.Next(1, 5),
                });
            }

            //UpdateDrawable Last
            base.Update();
        }

        public override void UpdateDrawable()
        {
            base.UpdateDrawable();
            BulletLayer.bSize[Drawable] = Size.X * Scale.X;
        }

        public virtual Vector2 GetPosition(double time)
        {
            double scale = Math.Clamp(PrionMath.Remap(time, StartTime, EndTime), 0, 1);
            float t = (float)Easing.ApplyEasing(SpeedEasing, scale);

            switch (CurveType)
            {
                default:
                    return new Vector2(PrionMath.Remap(t, 0, 1, StartPosition.X, EndPosition.X),
                        PrionMath.Remap(t, 0, 1, StartPosition.Y, EndPosition.Y));
                case CurveType.Bezier:
                    return PrionMath.Bezier(t, StartPosition, CurvePoint, EndPosition);
            }
        }

        public override void Start()
        {
            base.Start();

            if (!Vitaru.EnableKeyFrames)
                Alpha = 1;
        }

        public override void End()
        {
            if (ReturnedScore) return;
            base.End();

            ReturnGreat = false;
            ForceScore = true;

            if (!Vitaru.EnableKeyFrames)
                Alpha = 0;
            else
            {
                //Drawable.FadeTo(0, 250, Easings.InSine);
                //Drawable.ScaleTo(new Vector2(1.5f), 250, Easings.OutCubic).OnComplete(UnLoad);
            }
        }

        public override void ParseString(string[] data, int offset)
        {
            CircularHitbox.Diameter = float.Parse(data[0 + offset]);
            Speed = float.Parse(data[1 + offset]);

            base.ParseString(data, offset + 2);
        }

        public override string[] SerializeToStrings()
        {
            List<string> data = new()
            {
                "b",
                CircularHitbox.Diameter.ToString(),
                Speed.ToString(),
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

    public enum Shape
    {
        Circle,
        Triangle,
        Square,
    }
}