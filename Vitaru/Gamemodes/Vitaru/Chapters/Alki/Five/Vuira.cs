using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Vuira : Player
    {
        public override string Name => "Vuira";

        public override float HealthCapacity => 40;

        public override float EnergyCapacity => 36;

        public override float EnergyCost => 12;

        public override float EnergyDrainRate => 6;

        public override Color PrimaryColor => "#c3ad00".HexToColor();

        public override Color SecondaryColor => "#00cd00".HexToColor();

        public override Color ComplementaryColor => "#666666".HexToColor();

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override float SealRotationSpeed => 0.5f;

        public override string Ability => "Goddess of Singing";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Extreme;

        public override bool Implemented => false;

        public override string OriginMedia => "Alki.Five";

        public override string Description => "She is a little on the wild side when Muris is around.";

        public Vuira(Gamefield gamefield) : base(gamefield) { }
    }
}
