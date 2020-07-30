// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Ecstatic : Theme
    {
        public override Color PrimaryColor => new Vector3(0, 255, 90).Color255();
        public override Color SecondaryColor => new Vector3(255, 0, 90).Color255();
        public override Color TrinaryColor => new Vector3(90, 0, 255).Color255();
        public override Color QuadnaryColor => new Vector3(0, 90, 255).Color255();

        public override string Background => "Backgrounds\\ecstatic.png";
    }
}