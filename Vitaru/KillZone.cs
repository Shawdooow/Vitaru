// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Vitaru.Characters;
using Vitaru.Characters.Enemies;
using Vitaru.Characters.Players;

namespace Vitaru
{
    public sealed class KillZone : Layer2D<SpriteLayer>
    {
        public KillZone(Root root, Pack<Character> characters)
        {
            ParentScaling = Axes.Both;

            DrawablePlayer drawablePlayer = new DrawablePlayer();
            Player player = new Player(drawablePlayer);

            characters.Add(player);

            root.Add(player.InputHandler);

            DrawableEnemy drawableEnemy = new DrawableEnemy();
            Enemy enemy = new Enemy(drawableEnemy);

            characters.Add(enemy);

            Children = new SpriteLayer[]
            {
                drawableEnemy,
                drawablePlayer,
            };
        }
    }
}