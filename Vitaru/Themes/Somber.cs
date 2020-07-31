using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Somber : Ecstatic
    {
        public override Color PrimaryColor => new Vector3(0, 36, 100).Color255();
        public override Color SecondaryColor => new Vector3(36, 0, 100).Color255();

        public override string Background => "Backgrounds\\somber.png";
    }
}
