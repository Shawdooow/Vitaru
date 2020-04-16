// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Entitys;
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

        protected virtual DrawableCharacter Drawable { get; set; }

        public override void Update()
        {
        }
    }
}