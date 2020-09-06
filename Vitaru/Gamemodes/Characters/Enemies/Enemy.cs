// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Color;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Editor.Editables.Properties.Time;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Gamemodes.Projectiles.Patterns;
using Vitaru.Graphics.Particles;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Characters.Enemies
{
    public class Enemy : Character, IHasStartPosition, IHasStartTime, IHasColor//, IHasPatternID, IHasEndTime
    {
        public static int COUNT;

        public override string Name { get; set; } = nameof(Enemy);

        public const int ENEMY_TEAM = 0;

        public override float HitboxDiameter => 50f;

        public new DrawableEnemy Drawable;

        public override void SetDrawable(DrawableGameEntity drawable)
        {
            base.SetDrawable(drawable);
            Drawable = drawable as DrawableEnemy;
        }

        public override DrawableGameEntity GenerateDrawable()
        {
            return new DrawableEnemy(this);
        }

        public IDrawable2D GetOverlay(DrawableGameEntity draw) =>
            new Sprite(Game.TextureStore.GetTexture("Edit\\enemyOutline.png"))
            {
                Size = ((DrawableEnemy) draw).Sprite.Size,
                Scale = ((DrawableEnemy) draw).Sprite.Scale,
                Color = Color.Yellow
            };

        public EditableProperty[] GetProperties() => new EditableProperty[]
        {
            new EditableStartPosition(this),
            new EditableStartTime(this),
            new EditableColor(this),
        };

        public Color Color { get; set; } = ColorExtentions.RandomColor();

        public override Color PrimaryColor => global::Vitaru.Vitaru.ALKI ? Color.Magenta : Color;

        public override Color SecondaryColor => global::Vitaru.Vitaru.ALKI ? Color.MidnightBlue : Color.Red;

        public Vector2 StartPosition { get; set; }

        public virtual double StartTime { get; set; }

        public virtual double EndTime { get; protected set; } = double.MaxValue;

        public double Duration => EndTime - StartTime;

        public virtual double TimePreLoad => 600;

        public virtual double TimeUnLoad => TimePreLoad;

        public bool PreLoaded { get; private set; }

        public bool Started { get; private set; }

        private bool shoot = true;

        public Enemy(Gamefield gamefield) : base(gamefield)
        {
            Team = ENEMY_TEAM;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Position = StartPosition;
            COUNT++;
        }

        public override void OnNewBeat()
        {
            base.OnNewBeat();
            shoot = !shoot;
            if (shoot)
                ShootPlayer();
        }

        public override void Update()
        {
            base.Update();

            if (Clock.LastCurrent + TimePreLoad >= StartTime && Clock.LastCurrent < EndTime + TimeUnLoad && !PreLoaded)
                PreLoad();
            else if ((Clock.LastCurrent + TimePreLoad < StartTime || Clock.LastCurrent >= EndTime + TimeUnLoad) &&
                     PreLoaded)
                UnLoad();

            if (Clock.LastCurrent >= StartTime && Clock.LastCurrent < EndTime && !Started)
                Start();
            else if ((Clock.LastCurrent < StartTime || Clock.LastCurrent >= EndTime) && Started)
                End();
        }

        protected virtual void ShootPlayer()
        {
            Player player = (Player) Gamefield.PlayerPack.Children[0];

            float playerAngle =
                (float) Math.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);

            List<Projectile> projectiles;

            switch (PrionMath.RandomNumber(0, 5))
            {
                default:
                    projectiles = Patterns.Wave(0.25f, 28, 12, Position, Clock.LastCurrent, Team, 1, playerAngle);
                    break;
                case 1:
                    projectiles = Patterns.Line(0.5f, 0.25f, 28, 12, Position, Clock.LastCurrent, Team, 1, playerAngle);
                    break;
                case 2:
                    projectiles = Patterns.Triangle(0.25f, 28, 12, Position, Clock.LastCurrent, Team, 1, playerAngle);
                    break;
                case 3:
                    projectiles = Patterns.Wedge(0.25f, 28, 12, Position, Clock.LastCurrent, Team, 1, playerAngle);
                    break;
                case 4:
                    projectiles = Patterns.Circle(0.25f, 28, 12, Position, Clock.LastCurrent, Team);
                    break;
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Color = PrimaryColor;
                Gamefield.Add(projectile);
            }
        }

        protected virtual void PreLoad() => PreLoaded = true;

        protected virtual void Start() => Started = true;

        protected virtual void End() => Started = false;

        protected virtual void UnLoad() => PreLoaded = false;

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
                    Scale = 0.5f / PrionMath.RandomNumber(1, 4)
                });
            }

            Drawable?.Delete();
            Gamefield.Remove(this);
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
            HitboxDiameter = float.Parse(data[4 + offset]);
        }

        public virtual string[] SerializeToStrings()
        {
            return new[]
            {
                StartTime.ToString(),
                EndTime.ToString(),
                $"{StartPosition.X},{StartPosition.Y}",
                HitboxDiameter.ToString()
            };
        }
    }
}