// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Utilities;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Entitys;
using Vitaru.Graphics.Particles;

namespace Vitaru.Play
{
    public abstract class GameEntity : Updatable, IHasTeam
    {
        public override string Name { get; set; } = nameof(GameEntity);

        //0 = Enemies, 1 = Player, 2 = Enemy Players
        public virtual int Team { get; set; }

        public virtual bool Active { get; set; }

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

        public virtual Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                if (Drawable != null)
                    Drawable.Size = value;
            }
        }

        private Vector2 size;

        public virtual Vector2 Scale
        {
            get => scale;
            set
            {
                scale = value;
                if (Drawable != null)
                    Drawable.Scale = value;
            }
        }

        private Vector2 scale;

        public virtual float Alpha
        {
            get => alpha;
            set
            {
                alpha = value;
                if (Drawable != null)
                    Drawable.Alpha = value;
            }
        }

        private float alpha;

        public virtual Color Color
        {
            get => color;
            set
            {
                color = value;
                if (Drawable != null)
                    Drawable.Color = value;
            }
        }

        private Color color;

        public abstract Hitbox GetHitbox();

        public Action<Particle> OnAddParticle;

        protected DrawableGameEntity Drawable;

        public virtual void SetDrawable(DrawableGameEntity drawable)
        {
            Debugger.Assert(!Disposed, $"Can't set {nameof(Drawable)} of a {nameof(Disposed)} {nameof(GameEntity)}");
            Debugger.Assert(Drawable == null, $"{nameof(Drawable)} should be null");
            Drawable = drawable;
        }

        public abstract DrawableGameEntity GenerateDrawable();

        /// <summary>
        ///     Called by Update Thread
        /// </summary>
        public virtual void UpdateDrawable()
        {
            Drawable.Position = Position;
            Drawable.Size = Size;
            Drawable.Scale = Scale;
            Drawable.Alpha = Alpha;
            Drawable.Color = Color;
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Drawable?.Dispose();
            Drawable = null;
        }
    }
}