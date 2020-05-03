// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Graphics;
using Vitaru.Roots.Tests;

//using Prion.Integrations.Discord;
//using Prion.Integrations.Discord.DiscordGameSDK;

namespace Vitaru
{
    public class Vitaru : Game
    {
        public static int RANDOM { get; private set; }

        public static void Main(string[] args)
        {
            RANDOM = PrionMath.RandomNumber(0, 10);
            using (Vitaru vitaru = new Vitaru(args))
                vitaru.Start(new PlayTest());
        }

        protected Vitaru(string[] args) : base("vitaru", args, "GL46")
        {
            //DiscordGame.Init(700855485129162824);
            ////DiscordRich.RegisterUriPath();
            //DiscordGame.SetPresence(new Activity
            //{
            //    Details = "Preparing to Play...",
            //    State = "Main Menu",
            //    Assets = new ActivityAssets
            //    {
            //        LargeImage = "tau",
            //        LargeText = "Tau",
            //        SmallImage = "prion",
            //        SmallText = "Debugging...",
            //    },
            //    Instance = true,
            //});
        }

        public override void Start()
        {
            Renderer.Window.Title = RANDOM == 5 ? "Alki" : "Vitaru";
            Renderer.Window.Icon = new Icon(AssetStorage.GetStream(RANDOM == 5 ? "Textures\\alki.ico" : "Textures\\vitaru.ico"));
            base.Start();
        }

        protected override void Update()
        {
            //DiscordGame.RunCallbacks();
            base.Update();
        }
    }
}