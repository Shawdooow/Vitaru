﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Gamemodes.Characters
{
    public abstract class DrawableCharacter : SpriteLayer
    {
        public readonly Sprite Sprite;

        public readonly Circle Hitbox;

        protected DrawableCharacter(Character character, Texture t)
        {
            Add(Sprite = new Sprite(t));
            Name = Sprite.Name;
            Size = Sprite.Size;

            Add(Hitbox = new Circle
            {
                Size = new Vector2(character.HitboxDiameter),
                Alpha = 0,
            });
        }

        public event Action OnDelete;

        /// <summary>
        ///     Tells this <see cref="DrawableCharacter" /> to remove itself from our Parent and Dispose
        /// </summary>
        public virtual void Delete() => OnDelete?.Invoke();
    }
}