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
        private readonly Gamefield gamefield;

        //private readonly VitaruServerNetHandler vitaruServer;
        //private readonly VitaruNetHandler vitaruNet;

        public MainMenuRoot()
        {
            //string address = "127.0.0.1:36840";
            //vitaruServer = new VitaruServerNetHandler
            //{
            //    Address = address
            //};
            //vitaruNet = new VitaruNetHandler
            //{
            //    Address = address
            //};

            gamefield = new Gamefield();

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("Backgrounds\\medicine.png"))
                    {
                        Scale = new Vector2(2f),
                    },
                    new Sprite(Game.TextureStore.GetTexture("Backgrounds\\vitaru spring 2018.png"))
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

            //Add(new Pack<Updatable>
            //{
            //    Children = new Updatable[]
            //    {
            //        vitaruServer,
            //        vitaruNet,
            //    }
            //});

            //Packs
            Add(gamefield);
            Add(gamefield.PlayerPack);
            Add(gamefield.LoadedEnemies);
            Add(gamefield.ProjectilePack);

            //Layers
            Add(gamefield.ProjectileLayer);
            Add(gamefield.CharacterLayer);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            //vitaruNet.Connect();
            //vitaruNet.Ping();
        }

        public override void PreRender()
        {
            base.PreRender();
            gamefield.PreRender();
        }
    }
}