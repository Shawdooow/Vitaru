// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Chapters;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.One;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki
{
    public class AlkiChapter : Chapter
    {
        public override string Title => "The Alki Chapter";

        public override string Description => null;

        public override Player[] GetPlayers(Gamefield gamefield = null) => new Player[]
        {
            new Arysa(gamefield),
            new Tyle(gamefield),
            new Alice(gamefield),
            new Sarah(gamefield),
            new Yuie(gamefield),
            new Frost(gamefield)
        };
    }
}