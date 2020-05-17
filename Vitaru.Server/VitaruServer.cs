// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Core;

namespace Vitaru.Server
{
    public class VitaruServer : Application
    {
        public static void Main(string[] args)
        {
            using (VitaruServer server = new VitaruServer(args))
                server.Start();
        }

        protected VitaruServer(string[] args) : base("vitaru", args)
        {
        }
    }
}