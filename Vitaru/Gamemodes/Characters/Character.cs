// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Characters
{
    public abstract class Character : GameEntity
    {
        public override string Name { get; set; } = nameof(Character);

        public virtual float HealthCapacity => 60f;

        public virtual float Health { get; protected set; }

        public virtual float HitboxDiameter { get; protected set; } = 10;

        public bool Dead { get; protected set; }

        protected new DrawableCharacter Drawable;

        public override void SetDrawable(DrawableGameEntity drawable)
        {
            base.SetDrawable(drawable);
            Drawable = drawable as DrawableCharacter;
        }

        public virtual Color PrimaryColor => Color.Green;

        public virtual Color SecondaryColor => Color.LightBlue;

        public virtual Color ComplementaryColor => Color.LightGreen;

        protected Gamefield Gamefield { get; private set; }

        public Action OnDie;

        protected Character(Gamefield gamefield)
        {
            Gamefield = gamefield;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Health = HealthCapacity;
        }

        public virtual void OnNewBeat()
        {
        }

        public override void Update()
        {
            if (!Dead)
            {
                foreach (Gamefield.ProjectilePack pack in Gamefield.ProjectilePacks)
                {
                    if (pack.Team == Team) continue;

                    IReadOnlyList<Projectile> projectiles = pack.Children;
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        Projectile projectile = projectiles[i];

                        //Hack to disable bullets we shouldn't interact with
                        if (!projectile.Active)
                            continue;

                        ParseProjectile(projectile);

                        //TODO: Perform 1D calcs here to skip if possible
                        //if (projectile.Position.X > Position.X)
                        //{
                        //
                        //}
                        //else
                        //{
                        //    
                        //}
                        //
                        //if (projectile.Position.Y > Position.Y)
                        //{
                        //
                        //}
                        //else
                        //{
                        //
                        //}

                        float distance = Vector2.Distance(projectile.Position, Position);
                        float edgeDistance;

                        switch (projectile)
                        {
                            default:
                                continue;
                            case Bullet bullet:
                                edgeDistance = distance - (bullet.Diameter / 2 + HitboxDiameter / 2);
                                break;
                        }

                        if (edgeDistance <= 0)
                        {
                            Collision(projectile);
                            if (Dead) return;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets called just before hit detection
        /// </summary>
        protected virtual void ParseProjectile(Projectile projectile)
        {
        }

        protected virtual void Collision(Projectile projectile)
        {
            TakeDamage(projectile.Damage);
            Gamefield.Remove(projectile);
            projectile.Collision();
        }

        protected virtual void Heal(float amount)
        {
            Health = Math.Clamp(Health + amount, 0, HealthCapacity);
        }

        protected virtual void TakeDamage(float amount)
        {
            Health = Math.Clamp(Health - amount, 0, HealthCapacity);
            if (Health <= 0) Die();
        }

        protected virtual void BulletAddRad(float speed, float angle, Color color, float size, float damage,
            float distance)
        {
            Bullet bullet = new Bullet
            {
                Team = Team,
                StartPosition = Position,
                StartTime = Clock.Current,

                Speed = speed,
                Angle = angle,
                Color = color,
                Diameter = size,
                Damage = damage,
                Distance = distance
            };

            Gamefield.Add(bullet);
        }

        protected virtual void Die()
        {
            Dead = true;
            OnDie?.Invoke();
        }

        protected virtual void Rezzurect()
        {
        }

        protected override void Dispose(bool finalize)
        {
            Gamefield = null;
            Drawable = null;
            base.Dispose(finalize);
        }
    }
}