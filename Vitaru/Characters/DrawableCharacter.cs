// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Characters
{
    public abstract class DrawableCharacter : SpriteLayer
    {
        protected readonly Sprite Sprite;

        protected DrawableCharacter(Texture t)
        {
            Add(Sprite = new Sprite(t));
            Name = Sprite.Name;
            Size = Sprite.Size;
        }

        public event Action OnDelete;

        /// <summary>
        /// Tells this <see cref="DrawableCharacter" /> to remove itself from our Parent and Dispose
        /// </summary>
        public virtual void Delete() => OnDelete?.Invoke();
    }
}