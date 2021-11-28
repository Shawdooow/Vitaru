// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Chapters;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five;
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

        public override Player[] GetPlayers(Gamefield gamefield = null) => global::Vitaru.Vitaru.EnableCharacters
            ? new Player[]
            {
                new Arysa(gamefield),
                new Tyle(gamefield),
                new Alice(gamefield),
                new Sarah(gamefield),
                new Claire(gamefield),
                new Yuie(gamefield),
                new Frost(gamefield),
                new Jack(gamefield),
                new Nick(gamefield),
                new Muris(gamefield),
                new Vuira(gamefield),
                new Lucifer(gamefield),
                new Cuiria(gamefield),
            }
            : new Player[]
            {
                new Tyle(gamefield),
                new Alice(gamefield),
                new Claire(gamefield),
                new Yuie(gamefield),
            };
    }
}