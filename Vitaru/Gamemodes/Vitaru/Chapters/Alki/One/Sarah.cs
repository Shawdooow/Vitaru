// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

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

        private const int maxCharges = 4;

        private int charges;

        private const double chargeTime = 400;

        private double nextCharge;

        private double lastMovement;

        #endregion

        public Sarah(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);
            nextCharge = Gamefield.Current + chargeTime;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (Gamefield.Current >= nextCharge && charges < maxCharges)
            {

            }

            if (DrawablePlayer != null) DrawablePlayer.Seal.LeftValue.Text = $"{charges}x";
        }

        protected override Vector2 GetPositionOffset(double playerSpeed)
        {
            Vector2 position = base.GetPositionOffset(playerSpeed);

            if (Position != position) lastMovement = Gamefield.Current;

            return position;
        }
    }
}