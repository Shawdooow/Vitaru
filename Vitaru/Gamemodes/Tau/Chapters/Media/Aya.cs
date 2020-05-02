// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Gamemodes.Tau.Chapters.Media
{
    public class Aya : TouhosuPlayer
    {
        public override string Name => "Aya Shameimaru";

        public override double MaxHealth => 40;

        public override double MaxEnergy => 16;

        public override double EnergyCost => 4;

        //public override Color4 PrimaryColor => Color4.Red;

        //public override Color4 SecondaryColor => Color4.White;

        //public override Color4 TrinaryColor => Color4.Yellow;

        public override string Ability => "Snapshot";

        public override Role Role => Role.Offense;

        public override Ruleset.Characters.TouhosuPlayers.Difficulty Difficulty =>
            Ruleset.Characters.TouhosuPlayers.Difficulty.Normal;

        public override string Background => "";

        public override bool Implemented => false;
    }
}