// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Entitys;

namespace Vitaru.Play
{
    /// <summary>
    /// Wrapper for Game Drawables
    /// </summary>
    public abstract class DrawableGameEntity : Disposable
    {
        public override string Name { get; set; } = nameof(DrawableGameEntity);

        public abstract Vector2 Position { get; set; }

        public abstract Vector2 Size { get; set; }

        public abstract Vector2 Scale { get; set; }

        public abstract float Alpha { get; set; }

        public abstract Color Color { get; set; }
    }
}