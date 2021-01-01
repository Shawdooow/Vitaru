// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus;

namespace Vitaru.Server
{
    public class VitaruServer : Application
    {
        public static void Main(string[] args)
        {
            using (VitaruServer server = new VitaruServer(args))
                server.Start();
        }

        private const string host =
#if true
            "VitaruDebug";
#else
            "Vitaru";
#endif

        protected VitaruServer(string[] args) : base(host, args)
        {
        }
    }
}