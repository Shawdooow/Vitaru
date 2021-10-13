using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Muris : Player
    {
        public override string Name => "Muris";

        public override float HealthCapacity => 80;

        public override Color PrimaryColor => "#ff0000".HexToColor();

        public override Color SecondaryColor => "#ffffff".HexToColor();

        public override Color ComplementaryColor => "#666666".HexToColor();

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override float SealRotationSpeed => 0.5f;

        public override string Ability => "God of Rhythm";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override bool Implemented => false;

        public override string OriginMedia => "Alki.Five";

        public override string Description => "";

        public Muris(Gamefield gamefield) : base(gamefield) { }
    }
}
