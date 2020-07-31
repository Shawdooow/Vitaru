// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Themes
{
    public static class ThemeManager
    {
        public static Theme Theme { get; internal set; } = new Ecstatic();

        public static Color PrimaryColor => Theme.PrimaryColor;
        public static Color SecondaryColor => Theme.SecondaryColor;
        public static Color TrinaryColor => Theme.TrinaryColor;
        public static Color QuadnaryColor => Theme.QuadnaryColor;

        /// <summary>
        ///     Get menu Background <see cref="Texture" />
        /// </summary>
        /// <returns></returns>
        public static Texture GetBackground() => Vitaru.TextureStore.GetTexture(Theme.Background);
    }
}