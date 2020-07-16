using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;

namespace Vitaru.Themes
{
    public class Default : Theme
    {
        public override Color PrimaryColor => new Vector3(255, 0, 90).Color255();
        public override Color SecondaryColor => new Vector3(0, 255, 90).Color255();
        public override Color ComplementaryColor => Color.White;

        public override string Background => "Backgrounds\\VitaruTouhosuModeTrue1920x1080.png";
    }
}
