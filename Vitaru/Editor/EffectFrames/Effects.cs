﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Projectiles;

namespace Vitaru.Editor.EffectFrames
{
    #region Enemy


    public abstract class EnemyEffect : Effect
    {
        protected readonly Enemy Enemy;

        public EnemyEffect(Enemy enemy)
        {
            Enemy = enemy;
        }
    }

    public class EnemyPositionEffect : EnemyEffect
    {
        protected readonly Vector2 Start;
        protected readonly Vector2 End;

        public EnemyPositionEffect(Enemy enemy, Vector2 start, Vector2 end) : base(enemy)
        {
            Start = start;
            End = end;
        }

        protected override void ApplyEffect(float current) =>
            Enemy.Position = PrionMath.Remap(current, 0, 1, Start, End);
    }


    #endregion


    #region Projectile


    public abstract class ProjectileEffect : Effect
    {
        protected readonly Projectile Projectile;

        public ProjectileEffect(Projectile projectile)
        {
            Projectile = projectile;
        }
    }


    #endregion


    public abstract class Effect : IHasName
    {
        public virtual string Name { get; set; } = nameof(Effect);

        public virtual Easings Easing { get; set; } = Easings.None;

        public virtual void Apply(float current) =>
            ApplyEffect(Prion.Nucleus.Utilities.Easing.ApplyEasing(Easing, current));

        protected abstract void ApplyEffect(float current);
    }
}