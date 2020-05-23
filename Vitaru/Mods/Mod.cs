// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.UserInterface;

namespace Vitaru.Mods
{
    public abstract class Mod
    {
        public virtual Button GetMenuButton() => null;

        public virtual Root GetRoot() => null;

        //public virtual ModSubSection GetSettings() => null;

        //public virtual WikiSet GetWikiSet() => null;

        public virtual void LoadingComplete()
        {
        }
    }
}