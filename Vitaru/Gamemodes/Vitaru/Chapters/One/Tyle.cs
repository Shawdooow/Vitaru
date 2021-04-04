// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.One
{
    public class Tyle : Player
    {
        #region Fields

        public override string Name => "Tyle ";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 12;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 8;

        //public override Color PrimaryColor => "#a1e4ff".HexToColor();

        //public override Color SecondaryColor => "#009ad9".HexToColor();

        //public override Color ComplementaryColor => "#c2c2c2".HexToColor();

        public override string Ability => "Shadow Skipper";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Hard;

        #endregion

        public Tyle(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}