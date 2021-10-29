using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three
{
    public class Nick : Player
    {
        public override string Name => "Santa";

        //public override string Ability => "Cookies and Milk!";

        //public override Role Role => Role.Specialized;

        public Nick(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}
