// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Gamemodes.Characters
{
    public abstract class DrawableCharacter : DrawableGameEntity
    {
        public override string Name { get; set; } = nameof(DrawableCharacter);

        public readonly Sprite Sprite;

        public readonly Circle Hitbox;
        public readonly Circle HitboxOutline;

        protected DrawableCharacter(Character character, Texture t)
        {
            Add(Sprite = new Sprite(t));

            Name = Sprite.Name;
            Size = Sprite.Size;

            //TODO: get rid of this hacky shit
            if (character == null) return;

            Position = character.Position;

            Add(HitboxOutline = new Circle
            {
                Color = character.PrimaryColor,
                Size = new Vector2(character.Hitbox.Diameter * 1.75f),
                Alpha = 0
            });
            Add(Hitbox = new Circle
            {
                Size = new Vector2(character.Hitbox.Diameter),
                Alpha = 0
            });
        }
    }
}