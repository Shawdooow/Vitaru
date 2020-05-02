// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Chapters;
using Vitaru.Gamemodes.Tau.Chapters.Worship.Characters;
using Vitaru.Gamemodes.Tau.Chapters.Worship.Characters.Drawables;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Worship
{
    public class WorshipChapter : TouhosuChapter
    {
        public override string Title => "The Chapter of Worship";

        public override TouhosuPlayer[] GetTouhosuPlayers() => new[]
        {
            new Reimu(),
            new Ryukoy(),
            new Tomaji()
        };

        public override DrawableTouhosuPlayer GetDrawableTouhosuPlayer(VitaruPlayfield playfield, TouhosuPlayer player)
        {
            switch (player.Name)
            {
                default:
                    return null;

                case "Reimu Hakurei":
                    return new DrawableReimu(playfield);
                case "Ryukoy Hakurei":
                    return new DrawableRyukoy(playfield);
                case "Tomaji Hakurei":
                    return new DrawableTomaji(playfield);
            }
        }
    }
}