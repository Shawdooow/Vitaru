// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three
{
    public class Frost : Player
    {
        public override string Name => "Frost Pine";

        //public override float HealthCapacity => 60;

        //public override float EnergyCapacity => 24;

        //public override float EnergyCost => 4;

        //public override float EnergyDrainRate => 4;

        public override Color PrimaryColor => "#a1e4ff".HexToColor();

        public override Color SecondaryColor => "#009ad9".HexToColor();

        public override Color ComplementaryColor => "#c2c2c2".HexToColor();

        public override float SealRotationSpeed => -base.SealRotationSpeed;

        public override string Ability => "Absolute Zero";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override string OriginMedia => "Alki.Three";

        public Frost(PlayManager manager) : base(manager) { }
    }
}