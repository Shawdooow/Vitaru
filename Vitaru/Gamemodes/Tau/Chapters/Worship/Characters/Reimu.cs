// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Gamemodes.Tau.Chapters.Worship.Characters
{
    public class Reimu : TouhosuPlayer
    {
        public override string Name => "Reimu Hakurei";

        public override double MaxHealth => 60;

        public override double MaxEnergy => 24;

        public override double EnergyCost => 4;

        public override double EnergyDrainRate => 2;

        public override Color4 PrimaryColor => Color4.Red;

        public override Color4 SecondaryColor => Color4.White;

        public override Color4 TrinaryColor => Color4.Yellow;

        //TODO: Can collect ethereal clusters of various essence giving you and your teamates buffs
        public override string Ability => "Ethereal Support";

        #region Ability Values

        public override string AbilityStats => "";

        public const double BUFF_DURATION = 8;

        #endregion

        public override Role Role => Role.Support;

        public override Ruleset.Characters.TouhosuPlayers.Difficulty Difficulty =>
            Ruleset.Characters.TouhosuPlayers.Difficulty.Insane;

        public override string Background =>
            "      When she was young Reimu feared the idea of having kids after how much trouble her mother would tease she was. " +
            "However age has a funny way of changing peoples views. " +
            "One day she met Vaster's brother and the rest. . . the rest is history.\n\n" +
            "       Twenty years later despite being a widow she still has her two beautiful children, a girl and a boy. " +
            "The girl reminds her of herself, while the boy of him. " +
            "Ever since he died it has been quiet, Reimu has never known the forest to be so quiet. " +
            "Their shrine has remained uncontested for nearly sixteen years now, " +
            "but it still made her uneasy that soon her oldest would be taking over. " +
            "Ryukoy was by no means a fighter, if she ever got into trouble it likely wouldn't end well.\n\n" +
            "       All Reimu can do is pray, age not only changes your perspective but also your body. " +
            "She might have been highly regarded by many back in the day, but she certainly is not capable of what she used to be anymore.";

        public override bool Implemented => false;
    }
}