// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Gamemodes.Tau.Chapters.Worship.Characters
{
    public class Ryukoy : TouhosuPlayer
    {
        public override string Name => "Ryukoy Hakurei";

        public override double MaxHealth => 50;

        public override double MaxEnergy => 20;

        public override double EnergyCost => 10;

        public override double EnergyDrainRate => 0;

        public override Color4 PrimaryColor => Color4.Violet;

        public override Color4 SecondaryColor => base.SecondaryColor;

        public override Color4 TrinaryColor => base.TrinaryColor;

        public override string Ability => "Out of Tune";

        #region Ability Values

        public override string AbilityStats => "-Player drain: " + PLAYER_DRAIN_MULTIPLIER +
                                               "\n-Enemy drain: " + ENEMY_DRAIN_MULTIPLIER +
                                               "\n-Boss drain: " + BOSS_DRAIN_MULTIPLIER;

        public const double PLAYER_DRAIN_MULTIPLIER = 5d;

        public const double BULLET_ENERGY_COST = 2.5d;

        public const double ENEMY_DRAIN_MULTIPLIER = 1d;

        public const double BOSS_DRAIN_MULTIPLIER = 5d;

        #endregion

        public override Role Role => Role.Defense;

        public override Ruleset.Characters.TouhosuPlayers.Difficulty Difficulty =>
            Ruleset.Characters.TouhosuPlayers.Difficulty.Another;

        public override string Background =>
            "      Being the elder sibling comes with many responsabilitys in the Hakurei family. " +
            "She has the weight of the Hakurei name to uphold as the next inline to be the keeper of their family shrine. " +
            "Her mother would tell her stories about her adventures with her friends to stop the evil fairies from claiming it, and how they always would succeed. " +
            "One day she would like to go on an adventure of her own she would think. \"Becareful what you wish for\" Reimu would tell her.\n\n" +
            "       Now that she is almost a legal adult she has a very different view however, she is calm and level headed. " +
            "She doesn't actively seek trouble to solve or ways to cause trouble, she simply wishes for peace, quiet and an easy life as the next Hakurei Maiden.";

        public override bool Implemented => false;
    }
}