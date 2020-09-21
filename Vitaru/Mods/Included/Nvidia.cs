// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using System.Xml;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Timing;
using Vitaru.Roots;

namespace Vitaru.Mods.Included
{
    public class Nvidia : Mod
    {
        public override Button GetMenuButton() =>
            new Button
            {
                Y = 60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.LawnGreen
                },

                Text = "3080 Bot"
            };

        public override Root GetRoot() => new NvidiaRoot();

        private class NvidiaRoot : MenuRoot
        {
            private const string nvidia =
                @"https://api.digitalriver.com/v1/shoppers/me/products/5438481700/inventory-status?apiKey=9485fa7b159e42edb08a83bde0d83dia";

            private const string fail = "PRODUCT_INVENTORY_OUT_OF_STOCK";

            readonly XmlDocument doc = new XmlDocument();

            readonly Timer timer = new Timer
            {
                Interval = 2000f
            };

            public override void Update()
            {
                base.Update();

                if (timer.TimeElapsed(Clock))
                {
                    doc.Load(nvidia);

                    string t = doc.GetElementsByTagName("status").Item(0).InnerText;

                    if (t != fail)
                        Logger.SystemConsole("GO BUY AN RTX 3080!", ConsoleColor.Green);
                    else
                        Logger.SystemConsole("No RTX 3080s around right now...", ConsoleColor.DarkRed);
                }
            }
        }
    }
}