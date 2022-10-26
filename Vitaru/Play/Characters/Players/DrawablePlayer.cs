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
            Texture = Game.TextureStore.GetTexture("Gameplay\\player.png");
            CharacterSprite.Scale = new Vector2(0.3f);

            Color = player.PrimaryColor;
            SecondaryColor = player.SecondaryColor;

            Diameter = player.CircularHitbox.Diameter;

            CharacterLayer.Add(Seal = new Seal(player), AddPosition.First);
        }
    }
}