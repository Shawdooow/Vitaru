// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.UI;

namespace Vitaru.Play.Mods
{
    public abstract class Mod
    {
        public virtual bool Disabled => false;

        public virtual Button GetMenuButton() => null;

        public virtual Root GetRoot() => null;

        //public virtual ModSubSection GetSettings() => null;

        //public virtual WikiSet GetWikiSet() => null;

        public virtual void LoadingComplete()
        {
        }
    }
}