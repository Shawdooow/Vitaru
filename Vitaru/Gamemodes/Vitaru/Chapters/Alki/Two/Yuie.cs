// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two
{
    public class Yuie : Player
    {
        #region Fields

        public override string Name => "Yuie";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 32;

        public override float EnergyCost => 16;

        public override Color PrimaryColor => "#00ffbb".HexToColor();

        public override Color SecondaryColor => "#b0b0b0".HexToColor();

        public override Color ComplementaryColor => "#3d2a69".HexToColor();

        public override string Ability => "Snap Shot";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Normal;

        #endregion

        public Yuie(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}