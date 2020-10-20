// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters
{
    public class AlkiChapter : Chapter
    {
        public override string Title => "The Alki Chapter";

        public override string Description => null;

        public override Player[] GetPlayers(Gamefield gamefield = null) => new Player[]
        {
            new Sakuya(gamefield),
        };
    }
}