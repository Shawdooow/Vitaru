// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Utilities;
using System.Drawing;
using System.Numerics;

namespace Vitaru.Themes
{
    public class Rhize : Alki
    {
        public override Color PrimaryColor => new Vector3(39, 45, 70).Color255();
        public override Color SecondaryColor => new Vector3(95, 48, 84).Color255();

        public override string Background => "Backgrounds\\rhize.png";
    }
}