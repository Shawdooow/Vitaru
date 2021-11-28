// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Somber : Ecstatic
    {
        public override Color PrimaryColor => new Vector3(0, 36, 100).Color255();
        public override Color SecondaryColor => new Vector3(36, 0, 100).Color255();
        public override Color TrinaryColor => new Vector3(60, 60, 60).Color255();
        public override Color QuadnaryColor => new Vector3(180, 180, 180).Color255();

        public override string Background => "Backgrounds\\somber.png";
    }
}