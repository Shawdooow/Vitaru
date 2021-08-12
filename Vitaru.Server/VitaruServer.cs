// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Prion.Nucleus;
using Vitaru.Server.Levels;
using Vitaru.Server.Match;
using Vitaru.Server.Server;

namespace Vitaru.Server
{
    public class VitaruServer : Application
    {
        public const string VERSION = "0.12.0-preview4.0";

        private const string host =
#if true
            "VitaruServerDebug";
#else
            "VitaruServer";
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

        private void test()
        {
            MatchInfo m = new()
            {
                Name = "Test Match",
                ID = 36840,
                Host = 15,
                Users = new List<VitaruUser>
                {
                    new VitaruUser
                    {
                        Username = "Carl",
                        ID = 1,
                        Color = "#white",
                        UserSettings = new List<Setting>
                        {
                            new Setting
                            {
                                Name = "Character",
                                Value = "Arysa",
                                Sync = Sync.Client,
                            },
                            new Setting
                            {
                                Name = "Hard?",
                                Value = "Bab",
                                Sync = Sync.All,
                            },
                        },
                        Status = PlayerStatus.SearchingForLevel
                    },
                    new VitaruUser
                    {
                        Username = "Weeb",
                        ID = 2,
                        Color = "#green",
                        UserSettings = new List<Setting>
                        {
                            new Setting
                            {
                                Name = "Character",
                                Value = "Nobody",
                                Sync = Sync.Client,
                            },
                            new Setting
                            {
                                Name = "Easy",
                                Value = "yet",
                                Sync = Sync.All,
                            },
                        },
                        Status = PlayerStatus.DownloadingLevel
                    },
                },
            
                Settings = new List<Setting>
                {
                    new Setting
                    {
            
                    }
                },
            
                Level = new Level
                {
            
                }
            };
            
            byte[] data = m.Serialize();
            
            m.DeSerialize(data);
        }
    }

    public enum VitaruPackets
    {
        RequestMatchList = 101,
        MatchList,
        CreateMatch,
        MatchCreated,
        JoinMatch,
        JoinedMatch,
    }
}