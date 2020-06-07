// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Vitaru.Gamemodes;
using Vitaru.Play;

namespace Vitaru.Editor.IO
{
    public abstract class Editable
    {
        public abstract IEditable GetEditable(Gamefield field);

        public abstract IDrawable2D GetOverlay(DrawableGameEntity draw);
    }
}