using System.Drawing;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus;

namespace Vitaru.Roots.Menu
{
    public class Version : InstancedText
    {
        private const string version =
#if !PUBLISH
            Vitaru.VERSION;
#else
            $"DEBUG {Vitaru.VERSION}";
#endif

        public Version()
        {
            Y = -4;

            ParentOrigin = Mounts.BottomCenter;
            Origin = Mounts.BottomCenter;

            FontScale = 0.25f;

            Text = NucleusLaunchArgs.Features != Features.Standard
                ? $"{version} - {Vitaru.FEATURES}"
                : version;

            Color =
#if !PUBLISH
                Color.LimeGreen;
#else
                Color.Gold;
# endif
        }
    }
}
