// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Chapters;
using Vitaru.Gamemodes.Tau.Chapters.Media;
using Vitaru.Gamemodes.Tau.Chapters.Media.Drawables;
using Vitaru.Gamemodes.Tau.Chapters.Rational.Characters;
using Vitaru.Gamemodes.Tau.Chapters.Rational.Characters.Drawables;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Rational
{
    public class RationalChapter : TouhosuChapter
    {
        public override string Title => "The Rational Chapter";

        public override TouhosuPlayer[] GetTouhosuPlayers() => new[]
        {
            new Marisa(),
            new Aya()
        };

        public override DrawableTouhosuPlayer GetDrawableTouhosuPlayer(VitaruPlayfield playfield, TouhosuPlayer player)
        {
            switch (player.Name)
            {
                default:
                    return null;

                case "Marisa Kirisame":
                    return new DrawableMarisa(playfield);
                case "Aya Shameimaru":
                    return new DrawableAya(playfield);
            }
        }
    }
}