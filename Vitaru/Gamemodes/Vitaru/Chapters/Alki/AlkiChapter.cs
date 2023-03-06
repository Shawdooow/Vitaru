// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Gamemodes.Chapters;
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

        public override Player[] GetPlayers(PlayManager manager) => global::Vitaru.Vitaru.EnableIncompleteCharacters
            ? new Player[]
            {
                new Arysa(manager),
                new Tyle(manager),
                new Alice(manager),
                new Sarah(manager),
                new Claire(manager),
                new Yuie(manager),
                new Frost(manager),
                new Jack(manager),
                new Nick(manager),
                new Muris(manager),
                new Vuira(manager),
                new Lucifer(manager),
                new Cuiria(manager),
            }
            : new Player[]
            {
                new Tyle(manager),
                new Alice(manager),
                new Claire(manager),
                new Yuie(manager),
            };
    }
}