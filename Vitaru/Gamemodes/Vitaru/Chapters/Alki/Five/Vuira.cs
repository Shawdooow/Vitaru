using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Vuira : Player
    {
        public override string Name => "Vuira";

        public override float HealthCapacity => 80;

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override string Ability => "Goddess of Vocals";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.VeryHard;

        public override bool Implemented => false;

        public override string Description => "She is a little on the wild side when Muris is around.";

        public Vuira(Gamefield gamefield) : base(gamefield) { }
    }
}
