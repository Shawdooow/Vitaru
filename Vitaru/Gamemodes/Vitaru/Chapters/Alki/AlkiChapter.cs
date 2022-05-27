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

        public override Player[] GetPlayers() => global::Vitaru.Vitaru.EnableIncompleteCharacters
            ? new Player[]
            {
                new Arysa(),
                new Tyle(),
                new Alice(),
                new Sarah(),
                new Claire(),
                new Yuie(),
                new Frost(),
                new Jack(),
                new Nick(),
                new Muris(),
                new Vuira(),
                new Lucifer(),
                new Cuiria(),
            }
            : new Player[]
            {
                new Tyle(),
                new Alice(),
                new Claire(),
                new Yuie(),
            };
    }
}