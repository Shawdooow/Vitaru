// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;

namespace Vitaru.Themes
{
    public abstract class Theme
    {
        public abstract Color PrimaryColor { get; }
        public abstract Color SecondaryColor { get; }
        public abstract Color TrinaryColor { get; }
        public abstract Color QuadnaryColor { get; }

        public abstract string Background { get; }
    }
}