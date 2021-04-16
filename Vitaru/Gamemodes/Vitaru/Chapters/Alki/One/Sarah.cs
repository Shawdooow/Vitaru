// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Input;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Sarah : Player
    {
        #region Fields

        public override string Name => "Sarah";

        public override float HealthCapacity => 80;

        public override float EnergyCapacity => 32;

        public override float EnergyCost => 16;

        public override float EnergyDrainRate => 6;

        public override Color PrimaryColor => "#ffd000".HexToColor();

        public override Color SecondaryColor => "#2e2e2e".HexToColor();

        public override Color ComplementaryColor => "#3d16c9".HexToColor();

        public override string Ability => "Stardust";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Normal;

        #endregion

        public Sarah(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();
        }
    }
}