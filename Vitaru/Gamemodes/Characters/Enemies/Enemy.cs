// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Application.Utilities;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Characters.Enemies
{
    public class Enemy : Character, IEditable
    {
        public override string Name { get; set; } = nameof(Enemy);

        public const int ENEMY_TEAM = 0;

        public override float HitboxDiameter => 50f;

        public virtual DrawableEnemy GenerateDrawable()
        {
            DrawableEnemy draw = new DrawableEnemy(this);
            Drawable = draw;
            return draw;
        }

        public override Color PrimaryColor => global::Vitaru.Vitaru.ALKI ? Color.Magenta : Color.Chartreuse;

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
            const int numberbullets = 3;
            float directionModifier = -0.2f;

            Player player = (Player) Gamefield.PlayerPack.Children[0];

            float cursorAngle =
                ((float) Math.Atan2(player.Position.Y - Drawable.Position.Y, player.Position.X - Drawable.Position.X))
                .ToDegrees() + 90;

            for (int i = 1; i <= numberbullets; i++)
            {
                float size;
                //float damage;
                Color color;

                if (i % 2 == 0)
                {
                    size = 28;
                    //damage = 24;
                    color = PrimaryColor;
                }
                else
                {
                    size = 20;
                    //damage = 18;
                    color = SecondaryColor;
                }

                //-90 = up
                BulletAddRad(0.5f, (cursorAngle - 90).ToRadians() + directionModifier, color, size, 0);
                directionModifier += 0.2f;
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
            Drawable.Delete();
            Gamefield.Remove(this);
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