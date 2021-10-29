// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Jolly : Ecstatic
    {
        public new const string NAME = nameof(Jolly);
        public override string Name => NAME;

        public override Color PrimaryColor => new Vector3(0, 255, 0).Color255();
        public override Color SecondaryColor => new Vector3(255, 0, 0).Color255();
        public override Color TrinaryColor => new Vector3(0, 0, 255).Color255();
        public override Color QuadnaryColor => new Vector3(200, 200, 200).Color255();

        public override string Background => "Backgrounds\\jolly.png";
    }
}