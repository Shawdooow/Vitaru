using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three
{
    public class Jack : Player
    {
        public override string Name => "The Horseman";

        public override string Ability => "Pumpkin Chunkin'";

        public override Role Role => Role.Offense;

        public Jack(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}
