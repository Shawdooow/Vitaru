// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;

namespace Vitaru.Play.Characters.Enemies
{
    public class DrawableEnemy : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawableEnemy);

        public DrawableEnemy(Enemy enemy) : base(enemy, Game.TextureStore.GetTexture("Gameplay\\enemy.png"))
        {
            Scale = new Vector2(0.12f);
            Sprite.Color = enemy?.PrimaryColor ?? Color.White;
        }
    }
}