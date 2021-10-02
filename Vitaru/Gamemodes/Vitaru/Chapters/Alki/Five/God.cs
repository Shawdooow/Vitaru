using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class God : Player
    {
        public override string Name => "God";

        public override float HealthCapacity => 80;

        //public override string Seal => "Gameplay\\seal alki holy.png";

        public override float SealRotationSpeed => -base.SealRotationSpeed;

        public override string Ability => "Spirit Breaker";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override bool Implemented => false;

        public override string Description => "Is a complete bitch and we all hate her.";

        public God(Gamefield gamefield) : base(gamefield) { }
    }
}
