// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;

namespace Vitaru.Gamemodes
{
    public class DrawableGameEntity : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(DrawableGameEntity);

        public event Action OnDelete;

        /// <summary>
        ///     Tells this <see cref="DrawableGameEntity" /> to remove itself from our Parent and Dispose
        /// </summary>
        public virtual void Delete() => OnDelete?.Invoke();

        public override void Removed()
        {
            base.Removed();
            OnDelete = null;
        }
    }
}