// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;

namespace Vitaru.Characters.Enemies
{
    public class Enemy : Character
    {
        public const int ENEMY_TEAM = 0;

        public DrawableEnemy GenerateDrawable()
        {
            DrawableEnemy draw = new DrawableEnemy(this)
            {
                Y = -200f,
            };
            Drawable = draw;
            return draw;
        }

        public Enemy()
        {
            Team = ENEMY_TEAM;
        }

        public override void Update()
        {
            Drawable.Position = new Vector2(200 * MathF.Sin((float) Clock.Current / 500f), Drawable.Y);
        }
    }
}