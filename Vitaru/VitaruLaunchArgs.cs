// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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
            switch (pair.Key) { }
        }
    }
}