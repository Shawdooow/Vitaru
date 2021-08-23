// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Wiki
{
    public abstract class WikiSection : IHasName
    {
        public abstract string Name { get; }

        public abstract InputLayer<IDrawable2D> GetSection();
    }
}