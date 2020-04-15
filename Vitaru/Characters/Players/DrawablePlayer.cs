﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;

namespace Vitaru.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public DrawablePlayer() : base(Game.TextureStore.GetTexture("Sakuya Izayoi.png"))
        {
            Color = Color.Blue;
            Scale = new Vector2(0.5f);
        }
    }
}