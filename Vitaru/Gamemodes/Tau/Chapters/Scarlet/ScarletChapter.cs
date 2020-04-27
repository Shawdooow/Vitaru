// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Chapters;
using Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters;
using Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters.Drawables;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Scarlet
{
    public class ScarletChapter : TouhosuChapter
    {
        public override string Title => "The Chapter in Scarlet";

        public override TouhosuPlayer[] GetTouhosuPlayers() => new[]
        {
            new Remilia(),
            new Flandre(),
            new Sakuya()
        };

        public override DrawableTouhosuPlayer GetDrawableTouhosuPlayer(VitaruPlayfield playfield, TouhosuPlayer player)
        {
            switch (player.Name)
            {
                default:
                    return null;

                case "Sakuya Izayoi":
                    return new DrawableSakuya(playfield);
                case "Remilia Scarlet":
                    return new DrawableRemilia(playfield);
                case "Flandre Scarlet":
                    return new DrawableFlandre(playfield);
            }
        }
    }
}