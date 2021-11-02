using Prion.Nucleus.Utilities;
using System.Drawing;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three
{
    public class Nick : Player
    {
        public override string Name => "Santa";

        public override Color PrimaryColor => "#ff0000".HexToColor();

        public override Color SecondaryColor => "#00ff00".HexToColor();

        public override Color ComplementaryColor => "#005eff".HexToColor();

        public override float SealRotationSpeed => -base.SealRotationSpeed;

        public override string Ability => "Jolly Good Show!";

        public override Role Role => Role.Offense;

        public override string OriginMedia => "Alki.Three";

        public Nick(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}
