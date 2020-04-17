// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Characters.Enemies;
using Vitaru.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Roots
{
    public class MainMenuRoot : Root
    {
        private readonly Gamefield gamefield = new Gamefield();

        public MainMenuRoot()
        {
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

            Player player = new Player(gamefield);

            Add(player.InputHandler);

            gamefield.Add(player);
            gamefield.Add(new Enemy(gamefield)
            {
                StartTime = double.MinValue,
            });

            //Packs
            Add(gamefield);
            Add(gamefield.PlayerPack);
            Add(gamefield.LoadedEnemies);
            Add(gamefield.ProjectilePack);

            //Layers
            Add(gamefield.ProjectileLayer);
            Add(gamefield.CharacterLayer);
        }

        public override void PreRender()
        {
            base.PreRender();
            gamefield.PreRender();
        }
    }
}