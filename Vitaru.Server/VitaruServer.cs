// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus;

namespace Vitaru.Server
{
    public class VitaruServer : Application
    {
        private const string host =
#if true
            "VitaruDebug";
#else
            "Vitaru";
#endif

        public static void Main(string[] args)
        {
            NucleusLaunchArgs n = new()
            {
                Name = host
            };
            NucleusLaunchArgs.ProccessArgs(args);

            using (VitaruServer server = new(n))
                server.Start();
        }

        protected VitaruServer(NucleusLaunchArgs args) : base(args)
        {
        }
    }
}