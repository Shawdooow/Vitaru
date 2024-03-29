﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Chapters;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki;

namespace Vitaru.Gamemodes.Vitaru
{
    public class VitaruChapterset : Chapterset
    {
        public override string Name => "Vitaru";

        public override string Description =>
            "The movement gamemode, Vitaru is all about moving out of the way to the beat.";

        public override Chapter[] GetChapters() => new Chapter[]
        {
            new AlkiChapter(),
        };
    }
}