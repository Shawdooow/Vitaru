// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game;

namespace Vitaru.Gamemodes.Characters.Enemies
{
    public class DrawableEnemy : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawableEnemy);

        public DrawableEnemy(Enemy enemy) : base(enemy, Game.TextureStore.GetTexture("Gameplay\\enemy.png"))
        {
            Sprite.Color = enemy.PrimaryColor;
            Sprite.Scale = new Vector2(0.1f);
            Position = enemy.StartPosition;
        }
    }
}