// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Application.Groups.Packs;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Characters;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        public MainMenuRoot()
        {
            Renderer.Window.Title = "Vitaru";

            Pack<Character> characters = new Pack<Character>();

            Add(characters);

            Add(new SpriteLayer
            {
                ParentScaling = Axes.Both,

                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("vitaru spring 2018.png"))
                    {
                        ParentScaling = Axes.Both,
                        Scale = new Vector2(0.75f),
                    },
                    new Box
                    {
                        ParentScaling = Axes.Both,
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5),
                    },
                },
            });
            Add(new KillZone(this, characters));
        }
    }
}