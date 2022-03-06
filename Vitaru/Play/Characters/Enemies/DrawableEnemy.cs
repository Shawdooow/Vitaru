// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;

namespace Vitaru.Play.Characters.Enemies
{
    public class DrawableEnemy : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawableEnemy);

        public DrawableEnemy(Layer2D<IDrawable2D> layer) : base(layer)
        {
        }
    }
}