﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Input;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Alice : Player
    {
        #region Fields

        public override string Name => "Alice";

        public override float HealthCapacity => 20;

        //public override float EnergyCapacity => 24;

        //public override float EnergyCost => 16;

        public override Color PrimaryColor => "#fc0330".HexToColor();

        public override Color SecondaryColor => "#363636".HexToColor();

        public override Color ComplementaryColor => "#7da1a8".HexToColor();

        public override string Ability => "Unbound";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.SeriousShit;

        #endregion

        public Alice(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            Gamefield.Shade = Shades.Gray;
        }
    }
}