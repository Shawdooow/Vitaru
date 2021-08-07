// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Alice : Player
    {
        #region Fields

        public override string Name => "Alice";

        public override float HealthCapacity => 420;

        public override float EnergyCapacity => 69;

        public override float EnergyCost => 1;

        public override Color PrimaryColor => "#fc0330".HexToColor();

        public override Color SecondaryColor => "#363636".HexToColor();

        public override Color ComplementaryColor => "#7da1a8".HexToColor();

        public override string Ability => "Unbound";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.Impossible;

        public override bool AI => global::Vitaru.Vitaru.FEATURES >= Features.Radioactive;

        #endregion

        public Alice(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            Gamefield.Shade = Shades.Gray;
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            Gamefield.Shade = Shades.None;
        }
    }
}