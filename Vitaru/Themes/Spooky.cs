// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Spooky : Ecstatic
    {
        public const string NAME = nameof(Spooky);
        public override string Name => NAME;

        public override Color PrimaryColor => new Vector3(255, 84, 0).Color255();
        public override Color SecondaryColor => new Vector3(20, 20, 20).Color255();

        public override string Background => "Backgrounds\\spooky.png";
    }
}