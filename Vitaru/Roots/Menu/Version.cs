// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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
            "DEBUG " + Vitaru.VERSION;
#else
            Vitaru.VERSION;
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

#if !PUBLISH
            Color = Color.Red;
#else
            switch (NucleusLaunchArgs.Features)
            {
                case Features.Safe:
                    Color = Color.DeepSkyBlue;
                    break;
                default:
                    Color = Color.LimeGreen;
                    break;
                case Features.Upcoming:
                    Color = Color.Gold;
                    break;
                case Features.Experimental:
                    Color = Color.OrangeRed;
                    break;
                case Features.Radioactive:
                    Color = Color.Purple;
                    break;
            }
# endif
        }
    }
}