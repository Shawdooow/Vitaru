// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Groups;

namespace Vitaru.Play.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawablePlayer);

        public Seal Seal { get; }

        public DrawablePlayer(Player player, Layer2D<IDrawable2D> layer) : base(layer)
        {
            CharacterSprite.Texture = Game.TextureStore.GetTexture("Gameplay\\player.png");
            CharacterSprite.Color = player.PrimaryColor;
            CharacterSprite.Scale = new Vector2(0.3f);

            CharacterLayer.Add(Seal = new Seal(player), AddPosition.First);
        }
    }
}