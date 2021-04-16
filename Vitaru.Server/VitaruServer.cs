// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus;
using Vitaru.Server.Server;

namespace Vitaru.Server
{
    public class VitaruServer : Application
    {
        private const string host =
# true
            "VitaruServerDebug";
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

        private readonly VitaruServerNetHandler server;

        protected VitaruServer(NucleusLaunchArgs args) : base(args)
        {
            server = new VitaruServerNetHandler
            {
                Address = "127.0.0.1:36840"
            };
        }

        protected override void RunUpdate()
        {
            server.PreLoading();
            server.Clock = Clock;
            server.LoadingComplete();

            base.RunUpdate();
        }

        protected override void Update()
        {
            base.Update();
            server.Update();
        }

        public override void Dispose()
        {
            server.Dispose();
            base.Dispose();
        }
    }
}