﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities.Interfaces;
using Vitaru.Editor.Editables;
using Vitaru.Editor.IO;
using Vitaru.Play;

namespace Vitaru.Gamemodes
{
    public abstract class Gamemode : IHasName, IHasDescription, IHasIcon
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Gamemode.";

        public abstract Texture Icon { get; }

        public abstract FormatConverter GetFormatConverter();

        public abstract Gamefield GetGamefield();

        public abstract Chapter[] GetChapters();

        public abstract EditableGenerator[] GetGenerators();

        public virtual Vector2 GetGamefieldSize() => new Vector2(1024, 820);
    }
}