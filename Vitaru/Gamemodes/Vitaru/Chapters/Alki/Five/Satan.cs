﻿using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Satan : Player
    {
        public override string Name => "Satan";

        public override float HealthCapacity => 80;

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override string Ability => "God of the Damned";

        public override Role Role => Role.Defense;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override bool Implemented => false;

        public Satan(Gamefield gamefield) : base(gamefield) { }
    }
}