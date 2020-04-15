// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Entitys;

namespace Vitaru.Characters
{
    public abstract class Character : Updatable
    {
        public virtual float HealthCapacity => 60f;

        public virtual float Health { get; protected set; }

        public virtual float EnergyCapacity => 20f;

        public virtual float Energy { get; protected set; }

        protected readonly DrawableCharacter Drawable;

        protected Character(DrawableCharacter drawable)
        {
            Drawable = drawable;
        }

        public override void Update()
        {
        }
    }
}