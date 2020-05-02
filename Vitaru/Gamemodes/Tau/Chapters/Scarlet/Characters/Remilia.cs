// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters
{
    public class Remilia : TouhosuPlayer
    {
        public override string Name => "Remilia Scarlet";

        public override double MaxHealth => 60;

        public override double MaxEnergy => 12;

        public override double EnergyCost => 2;

        public override Color4 PrimaryColor => Color4.LightPink;

        public override Color4 SecondaryColor => Color4.White;

        public override Color4 TrinaryColor => Color4.Red;

        public override string Ability => "Vampuric";

        public override Role Role => Role.Offense;

        public override Ruleset.Characters.TouhosuPlayers.Difficulty Difficulty =>
            Ruleset.Characters.TouhosuPlayers.Difficulty.Normal;

        public override string Background => "";

        public override bool Implemented => false;
    }
}