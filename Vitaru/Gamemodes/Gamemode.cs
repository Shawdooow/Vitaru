// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities.Interfaces;
using System.Numerics;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Chapters;
using Vitaru.Wiki;

namespace Vitaru.Gamemodes
{
    public abstract class Gamemode : IHasName, IHasDescription, IHasIcon
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Gamemode.";

        public abstract Texture Icon { get; }

        public virtual WikiPanel GetWikiPanel() => null;

        public abstract FormatConverter GetFormatConverter();

        //public abstract Gamefield GetGamefield();

        public abstract Chapter[] GetChapters();

        public virtual Vector2 GetGamefieldSize() => new(1024, 820);
    }
}