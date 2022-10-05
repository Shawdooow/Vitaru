// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Cuiria : Player
    {
        public override string Name => "God";

        public override float HealthCapacity => 80;

        public override Color PrimaryColor => "#e7e7e7".HexToColor();

        public override Color SecondaryColor => "#ffff87".HexToColor();

        public override Color ComplementaryColor => "#9797dc".HexToColor();

        //public override string Seal => "Gameplay\\seal alki holy.png";

        public override float SealRotationSpeed => -base.SealRotationSpeed;

        public override string Ability => "Goddess of Creation";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override string OriginMedia => "Alki.Five";

        public override string Description => "Is a complete bitch and we all hate her.";

        public Cuiria(PlayManager manager) : base(manager) { }
    }
}