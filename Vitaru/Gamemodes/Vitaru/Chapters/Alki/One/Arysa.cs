// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Arysa : Player
    {
        public override string Name => "Arysa";

        public override float HealthCapacity => 80;

        public override float EnergyCapacity => 128;

        public override float EnergyCost => 128;

        public override float EnergyDrainRate => 8;

        public override Color PrimaryColor => "#00ff40".HexToColor();

        public override Color SecondaryColor => "#b3b3b3".HexToColor();

        public override Color ComplementaryColor => "#7aff9c".HexToColor();

        public override string Ability => "Crystaline";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Normal;
    }
}