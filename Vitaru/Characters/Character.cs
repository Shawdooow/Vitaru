// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Application.Debug;
using Prion.Application.Entitys;
using Vitaru.Play;
using Vitaru.Projectiles;
using Vitaru.Utilities;

namespace Vitaru.Characters
{
    public abstract class Character : Updatable, IHasTeam
    {
        //0 = Enemies, 1 = Player, 2 = Enemy Players
        public virtual int Team { get; set; }

        public virtual float HealthCapacity => 60f;

        public virtual float Health { get; protected set; }

        public virtual float EnergyCapacity => 20f;

        public virtual float Energy { get; protected set; }

        public virtual float HitboxDiameter { get; protected set; } = 10;

        protected virtual DrawableCharacter Drawable { get; set; }

        protected Gamefield Gamefield { get; private set; }

        protected Character(Gamefield gamefield)
        {
            Gamefield = gamefield;
        }

        public override void Update()
        {
            for (int i = 0; i < Gamefield.ProjectilePack.Children.Count; i++)
            {
                Projectile projectile = Gamefield.ProjectilePack.Children[i];

                if (projectile.Team == Team) continue;

                Vector2 difference = projectile.Position - Drawable.Position;

                double distance = Math.Sqrt(Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2));
                double edgeDistance = double.MaxValue;

                switch (projectile)
                {
                    default:
                        continue;
                    case Bullet bullet:
                        edgeDistance = distance - (bullet.Diameter / 2 + HitboxDiameter / 2);
                    break;
                }

                if (edgeDistance <= 0)
                    Logger.SystemConsole("HIT!!!!");
            }
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Gamefield = null;
            Drawable = null;
        }
    }
}