// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Wiki
{
    public abstract class WikiPanel : IHasName, IHasDescription, IHasIcon
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Wiki.";

        public virtual Texture Icon { get; }

        public abstract WikiSection[] GetSections();
    }
}
