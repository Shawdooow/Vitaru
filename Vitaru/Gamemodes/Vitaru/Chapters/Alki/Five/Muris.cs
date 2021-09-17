using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Muris : Player
    {
        public override string Name => "Murice";

        public override float HealthCapacity => 80;

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override string Ability => "God of Rhythm";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override bool Implemented => false;

        public Muris(Gamefield gamefield) : base(gamefield) { }
    }
}
