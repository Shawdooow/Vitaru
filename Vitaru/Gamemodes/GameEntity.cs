// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Application.Debug;
using Prion.Application.Entitys;
using Vitaru.Utilities;

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

        protected DrawableGameEntity Drawable;

        public virtual void SetDrawable(DrawableGameEntity drawable)
        {
            PrionDebugger.Assert(Drawable == null, "Drawable should be null");
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