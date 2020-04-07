// Copyright (c) 2018-2019 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Game;

namespace Vitaru
{
    public class Vitaru : Game
    {
        public static void Main(string[] args)
        {
            using (Vitaru vitaru = new Vitaru(args))
                vitaru.Start();
        }

        protected Vitaru(string[] args) : base("vitaru", args)
        {
        }
    }
}