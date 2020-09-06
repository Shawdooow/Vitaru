// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Golgi.Utilities;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Entitys;
using Vitaru.Graphics.Particles;

namespace Vitaru.Gamemodes
{
    public abstract class GameEntity : Updatable, IHasTeam
    {
        public override string Name { get; set; } = nameof(GameEntity);

        //0 = Enemies, 1 = Player, 2 = Enemy Players
        public virtual int Team { get; set; }

        public virtual Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                if (Drawable != null)
                    Drawable.Position = value;
            }
        }

        private Vector2 position;

        public Action<Particle> OnAddParticle;

        protected DrawableGameEntity Drawable;

        public virtual void SetDrawable(DrawableGameEntity drawable)
        {
            Debugger.Assert(!Disposed, $"Can't set {nameof(Drawable)} of a {nameof(Disposed)} {nameof(GameEntity)}");
            Debugger.Assert(Drawable == null, $"{nameof(Drawable)} should be null");
            Drawable = drawable;
        }

        public abstract DrawableGameEntity GenerateDrawable();

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Drawable = null;
        }
    }
}