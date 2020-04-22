﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public DrawablePlayer(Player player) : base(player, Game.TextureStore.GetTexture("Gameplay\\Sakuya Izayoi.png"))
        {
            Sprite.Color = player.PrimaryColor;
            Sprite.Scale = new Vector2(0.5f);
        }
    }
}