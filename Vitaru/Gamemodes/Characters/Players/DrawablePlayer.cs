// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawablePlayer);

        public Seal Seal { get; }

        public DrawablePlayer(Player player) : base(player, Game.TextureStore.GetTexture("Gameplay\\player.png"))
        {
            Sprite.Color = player.PrimaryColor;
            Sprite.Scale = new Vector2(0.3f);

            Add(Seal = new Seal(player), AddPosition.First);
        }
    }
}