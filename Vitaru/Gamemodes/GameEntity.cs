﻿using System.Numerics;
using Prion.Application.Debug;
using Prion.Application.Entitys;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
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

        public DrawableGameEntity GetDrawable()
        {
            PrionDebugger.Assert(Drawable == null, "Drawable should be null");
            return Drawable = GenerateDrawable();
        }

        protected abstract DrawableGameEntity GenerateDrawable();

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Drawable = null;
        }
    }
}
