using System.Collections.Generic;
using Prion.Mitochondria;

namespace Vitaru
{
    public class VitaruLaunchArgs : MitochondriaLaunchArgs
    {
        static VitaruLaunchArgs()
        {
            ParseLaunchArgs += parse;
        }

        private static void parse(KeyValuePair<string, string> pair)
        {
            switch (pair.Key)
            {
                default:
                    break;
            }
        }
    }
}
