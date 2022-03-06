// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;

namespace Vitaru.Play.Characters
{
    public abstract class DrawableCharacter : DrawableGameEntity
    {
        public override string Name { get; set; } = nameof(DrawableCharacter);

        public override Vector2 Position { get; set; }

        public override Vector2 Size { get; set; }

        public override Vector2 Scale { get; set; }

        public override float Alpha { get; set; }

        public override Color Color { get; set; }
    }
}