// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Integrations.Discord;
using Prion.Integrations.Discord.DiscordGameSDK;
using Vitaru.Roots;

namespace Vitaru
{
    public class Vitaru : Game
    {
        public static void Main(string[] args)
        {
            using (Vitaru vitaru = new Vitaru(args))
                vitaru.Start(new MainMenuRoot());
        }

        protected Vitaru(string[] args) : base("vitaru", args, "GL46")
        {
            DiscordGame.Init(700855485129162824);
            //DiscordRich.RegisterUriPath();
            DiscordGame.SetPresence(new Activity
            {
                Details = "Preparing to Play...",
                State = "Main Menu",
                Assets = new ActivityAssets
                {
                    LargeImage = "tau",
                    LargeText = "Tau",
                    SmallImage = "prion",
                    SmallText = "Debugging...",
                },
            });
        }

        protected override void RunUpdate()
        {
            Renderer.Window.Title = "Vitaru";
            Renderer.Window.Icon = new Icon(AssetStorage.GetStream("Textures\\vitaru.ico"));
            base.RunUpdate();
        }
    }
}