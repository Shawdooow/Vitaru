// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;

namespace Vitaru.Characters.Enemies
{
    public class DrawableEnemy : DrawableCharacter
    {
        public DrawableEnemy() : base(Game.TextureStore.GetTexture("enemy.png"))
        {
            Sprite.Color = Color.Chartreuse;
            Sprite.Scale = new Vector2(0.1f);
        }
    }
}