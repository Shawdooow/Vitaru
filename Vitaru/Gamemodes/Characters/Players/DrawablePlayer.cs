// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawablePlayer);

        public Sprite SignSprite { get; private set; }

        public DrawablePlayer(Player player) : base(player, Game.TextureStore.GetTexture("Gameplay\\Sakuya Izayoi.png"))
        {
            Sprite.Color = player.PrimaryColor;
            Sprite.Scale = new Vector2(0.5f);

            Add(SignSprite = new Sprite(Game.TextureStore.GetTexture("Gameplay\\seal.png"))
            {
                Scale = new Vector2(0.3f),
                Alpha = 0.5f,
                Color = player.PrimaryColor,
            }, AddPosition.First);
        }
    }
}