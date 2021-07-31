// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities.Interfaces;
using Vitaru.Wiki;

namespace Vitaru.Mods
{
    public abstract class Mod : IHasName, IHasDescription, IHasIcon
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Mod.";

        public abstract Texture Icon { get; }

        public virtual bool Disabled => false;

        public virtual Button GetMenuButton() => null;

        public virtual Root GetRoot() => null;

        //public virtual ModSubSection GetSettings() => null;

        public virtual WikiPanel GetWikiPanel() => null;

        public virtual void LoadingComplete()
        {
        }
    }
}