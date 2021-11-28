// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Lucifer : Player
    {
        public override string Name => "Satan";

        public override float HealthCapacity => 80;

        public override Color PrimaryColor => "#d80000".HexToColor();

        public override Color SecondaryColor => "#6f0606".HexToColor();

        public override Color ComplementaryColor => "#bb8562".HexToColor();

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override string Ability => "God of the Damned";

        public override Role Role => Role.Defense;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override string OriginMedia => "Alki.Five";

        public Lucifer(Gamefield gamefield) : base(gamefield) { }
    }
}