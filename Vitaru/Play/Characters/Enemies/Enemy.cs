﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Color;
using Vitaru.Editor.Editables.Properties.Pattern;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Editor.Editables.Properties.Time;
using Vitaru.Editor.KeyFrames;
using Vitaru.Editor.KeyFrames.Interfaces;
using Vitaru.Graphics.Particles;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;

namespace Vitaru.Play.Characters.Enemies
{
    public class Enemy : Character, IHasKeyFrames, IHasStartPosition, IHasColor, IHasPatternID, IHasPosition, IHasAlpha
    {
        public static int COUNT;

        public const int ENEMY_TEAM = 0;

        public override string Name { get; set; } = nameof(Enemy);

        public override float HealthCapacity => MaxHealth;

        public float MaxHealth { get; set; } = 60;

        public new DrawableEnemy Drawable;

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                if (Drawable != null)
                {
                    double current = Clock.Current;
                    Drawable.Alpha = current + TimePreLoad >= StartTime && current < EndTime + TimeUnLoad && Selected
                        ? 1
                        : 0;
                }
            }
        }

        private bool selected;

        public override void SetDrawable(DrawableGameEntity drawable)
        {
            base.SetDrawable(drawable);
            Drawable = drawable as DrawableEnemy;
        }

        public override DrawableGameEntity GenerateDrawable() => new DrawableEnemy(this);

        public IDrawable2D GetOverlay(DrawableGameEntity draw) =>
            new Sprite(Game.TextureStore.GetTexture("Edit\\enemyOutline.png"))
            {
                Size = ((DrawableEnemy)draw).Sprite.Size,
                Scale = ((DrawableEnemy)draw).Sprite.Scale,
                Color = Color.Yellow,
            };

        public EditableProperty[] GetProperties() => new EditableProperty[]
        {
            new EditableStartPosition(this),
            new EditableStartTime(this),
            new EditableColor(this),
            new EditablePatternID(this),
        };

        public List<KeyValuePair<int, List<KeyFrame>>> KeyFrames { get; set; } = new();

        public Color Color { get; set; } = ColorExtensions.RandomColor();

        public override Color PrimaryColor => Vitaru.ALKI > 0
            ? Vitaru.ALKI == 2 ? Color.Red : Color.Magenta
            : Color;

        public override Color SecondaryColor => Vitaru.ALKI > 0
            ? Vitaru.ALKI == 2 ? Color.MidnightBlue : Color.CornflowerBlue
            : Color.Yellow;

        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value;
                if (Drawable != null)
                    Drawable.Alpha = alpha;
            }
        }

        private float alpha;

        public Vector2 StartPosition { get; set; }

        public virtual double StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                EndTime = StartTime + 200;
            }
        }

        private double startTime;

        public virtual double EndTime { get; set; } = double.MaxValue;

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public bool PreLoaded { get; private set; }

        public bool Started { get; private set; }

        public short PatternID { get; set; }

        public bool ShootPlayer { get; set; }

        public Enemy(Gamefield gamefield) : base(gamefield)
        {
            Team = ENEMY_TEAM;
            CircularHitbox.Diameter = 50;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            if (!Vitaru.EnableKeyFrames) Position = StartPosition;
            COUNT++;
        }

        public override void Update()
        {
            base.Update();

            double current = Clock.Current;

            if (Vitaru.EnableKeyFrames)
                for (int i = 0; i < KeyFrames.Count; i++)
                    KeyFrame.ApplyFrames(current, KeyFrames[i].Value);

            if (!Selected)
            {
                if (current + TimePreLoad >= StartTime && current < EndTime + TimeUnLoad && !PreLoaded)
                    PreLoad();
                else if ((current + TimePreLoad < StartTime || current >= EndTime + TimeUnLoad) &&
                         PreLoaded)
                    UnLoad();

                if (current >= StartTime && current < EndTime && !Started)
                    Start();
                else if ((current < StartTime || current >= EndTime) && Started)
                    End();
            }
        }

        protected override void Collision(Projectile projectile)
        {
            if (Drawable != null && Drawable.Alpha > 0)
                base.Collision(projectile);
        }

        protected virtual void PreLoad()
        {
            PreLoaded = true;

            Health = HealthCapacity;

            if (Vitaru.EnableKeyFrames) return;

            if (Drawable != null && Drawable.LoadState == LoadState.Loaded)
            {
                Drawable.ClearTransforms();
                Drawable.Alpha = 0;
                Drawable.FadeTo(1, TimePreLoad);
            }
        }

        protected virtual void Start()
        {
            Started = true;

            //if (Vitaru.EnableKeyFrames) return;

            Shoot();
        }

        protected virtual void End()
        {
            Started = false;

            if (Vitaru.EnableKeyFrames) return;

            if (Drawable != null && Drawable.LoadState == LoadState.Loaded)
            {
                Drawable.ClearTransforms();
                Drawable.Alpha = 1;
                Drawable.FadeTo(0, TimeUnLoad);
            }
        }

        protected virtual void UnLoad()
        {
            PreLoaded = false;

            if (Vitaru.EnableKeyFrames) return;

            if (Drawable != null && Drawable.LoadState == LoadState.Loaded)
            {
                Drawable.ClearTransforms();
                Drawable.Alpha = 0;
            }
        }

        protected override void Die()
        {
            base.Die();
            EndTime = Clock.LastCurrent;

            for (int i = 0; i < 100; i++)
            {
                float angle = ((float)PrionMath.RandomNumber(0, 360)).ToRadians();
                int distance = PrionMath.RandomNumber(80, 160);

                OnAddParticle?.Invoke(new Particle
                {
                    StartPosition = Position,
                    EndPosition = Position + PrionMath.Offset(distance, angle),
                    Color = Color.Vector(),
                    Scale = 0.5f / PrionMath.RandomNumber(1, 4),
                });
            }

            Drawable?.Delete();
            Gamefield.Remove(this);
        }

        protected virtual void Shoot()
        {
            float angle = MathF.PI / 2f;

            if (ShootPlayer)
            {
                Player player = (Player)Gamefield.PlayerPack.Children[0];
                angle = (float)Math.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
            }

            List<Projectile> projectiles;

            switch (PatternID)
            {
                default:
                    projectiles = Patterns.Wave(0.25f, 28, 12, Position, Clock.Current, Team, 1, angle);
                    break;
                case 1:
                    projectiles = Patterns.Line(0.5f, 0.25f, 28, 12, Position, Clock.Current, Team, 1,
                        angle);
                    break;
                case 2:
                    projectiles = Patterns.Triangle(0.25f, 28, 12, Position, Clock.Current, Team, 1,
                        angle);
                    break;
                case 3:
                    projectiles = Patterns.Wedge(0.25f, 28, 12, Position, Clock.Current, Team, 1,
                        angle);
                    break;
                case 4:
                    projectiles = Patterns.Circle(0.25f, 28, 12, Position, Clock.Current, Team);
                    break;
                case 5:
                    projectiles = Patterns.Spiral(0.5f, 28, 6, Position, Clock.Current, Duration, Team,
                        TrackManager.CurrentTrack.Metadata.GetBeatLength());
                    break;
                case 6:
                    projectiles = Patterns.Cross(0.5f, 28, 4, Clock.Current, Team);
                    break;
                case 7:
                    projectiles = Patterns.Swipe(0.5f, 28, 4, Clock.Current, Team);
                    break;
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.GlowColor = PrimaryColor;
                Gamefield.Add(projectile);
            }
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            COUNT--;
        }

        public virtual void ParseString(string[] data, int offset)
        {
            StartTime = double.Parse(data[0 + offset]);
            EndTime = double.Parse(data[1 + offset]);
            StartPosition = new Vector2(float.Parse(data[2 + offset]), float.Parse(data[3 + offset]));
            CircularHitbox.Diameter = float.Parse(data[4 + offset]);
        }

        public virtual string[] SerializeToStrings()
        {
            return new[]
            {
                StartTime.ToString(),
                EndTime.ToString(),
                $"{StartPosition.X},{StartPosition.Y}",
                CircularHitbox.Diameter.ToString(),
            };
        }
    }
}