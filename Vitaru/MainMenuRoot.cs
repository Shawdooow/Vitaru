// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Application.Groups.Packs;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Characters;
using Vitaru.Characters.Enemies;
using Vitaru.Characters.Players;
using Vitaru.Projectiles;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        private readonly SpriteLayer<DrawableBullet> bulletLayer = new SpriteLayer<DrawableBullet>();

        public MainMenuRoot()
        {
            Renderer.Window.Title = "Vitaru";

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("vitaru spring 2018.png"))
                    {
                        Scale = new Vector2(0.75f),
                    },
                    new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5),
                    },
                },
            });

            Pack<Character> characters = new Pack<Character>();

            Player player = new Player(bulletLayer);
            DrawablePlayer drawablePlayer = player.GenerateDrawable();

            Add(player.InputHandler);
            characters.Add(player);

            Enemy enemy = new Enemy();
            DrawableEnemy drawableEnemy = enemy.GenerateDrawable();

            characters.Add(enemy);

            Add(characters);
            Add(bulletLayer);
            Add(drawableEnemy);
            Add(drawablePlayer);
        }
    }
}