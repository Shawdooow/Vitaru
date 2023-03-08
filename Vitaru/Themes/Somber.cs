// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;

namespace Vitaru.Themes
{
    public class Somber : Ecstatic
    {
        public override Color PrimaryColor => base.TrinaryColor;
        public override Color SecondaryColor => base.QuadnaryColor;
        public override Color TrinaryColor => base.PrimaryColor;
        public override Color QuadnaryColor => base.SecondaryColor;

        public override string Background => "Backgrounds\\somber.png";
    }
}