// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Sprites;
using Vitaru.Chapters;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki;

namespace Vitaru.Gamemodes.Vitaru
{
    public class Vitaru : Gamemode
    {
        public override string Name => "Vitaru";

        public override Texture Icon { get; }

        public override FormatConverter GetFormatConverter() => new VitaruConverter();

        public override Chapter[] GetChapters() => new Chapter[]
        {
            new AlkiChapter(),
        };
    }
}