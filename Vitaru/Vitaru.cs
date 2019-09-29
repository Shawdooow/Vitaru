// Copyright (c) 2018-2019 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Game;
using Prion.Game.Graphics.Context;
using Vitaru.Screens;

namespace Vitaru
{
    public class AlkiOne : Game
    {
        public static void Main(string[] args)
        {
            using (AlkiOne alki = new AlkiOne(args))
                alki.Start(new MainMenu());
        }

        protected AlkiOne(string[] args) : base("vitaru", args)
        {
            //PrionWindow.Title = "Alki[1].Chapters[1]";
        }
    }
}