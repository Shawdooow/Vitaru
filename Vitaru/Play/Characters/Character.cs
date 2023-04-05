// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Vitaru.Play.Projectiles;

namespace Vitaru.Play.Characters
{
    public abstract class Character : GameEntity
    {
        public override string Name { get; set; } = nameof(Character);

        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                CircularHitbox.Position = value;
            }
        }

        protected readonly PlayManager PlayManager;

        public virtual float HealthCapacity => 60f;

        public virtual float Health { get; protected set; }

        public override Hitbox GetHitbox() => CircularHitbox;

        public CircularHitbox CircularHitbox = new()
        {
            Diameter = 10,
        };

        public virtual bool HitDetection { get; protected set; } = true;

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

        public Action OnDie;

        public Action OnRezzurect;

        public Character(PlayManager manager)
        {
            PlayManager = manager;

            Color = PrimaryColor;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            if (Health == 0) Health = HealthCapacity;
        }

        public virtual void OnNewBeat() { }

        /// <summary>
        ///     Gets called just before hit detection
        /// </summary>
        protected virtual void ParseProjectile(Projectile projectile) { }

        protected virtual void Collision(Projectile projectile) => TakeDamage(projectile.Damage);

        protected virtual void Heal(float amount)
        {
            Health = Math.Clamp(Health + amount, 0, HealthCapacity);
            if (Health >= HealthCapacity) Rezzurect();
        }

        protected virtual void TakeDamage(float amount)
        {
            Health = Math.Clamp(Health - amount, 0, HealthCapacity);
            if (Health <= 0) Die();
        }

        protected virtual void Die()
        {
            Dead = true;
            OnDie?.Invoke();
        }

        protected virtual void Rezzurect()
        {
            Dead = false;
            OnRezzurect?.Invoke();
        }

        protected override void Dispose(bool finalize)
        {
            Drawable = null;
            base.Dispose(finalize);
        }
    }
}