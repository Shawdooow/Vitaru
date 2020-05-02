// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters
{
    public class RejectChapter : Chapter
    {
        public override string Title => "The Rejected Chapter";

        public override string Description => null;

        public override Player[] GetPlayers(Gamefield gamefield = null)
        {
            throw new NotImplementedException();
        }

        //public override VitaruPlayer[] GetPlayers() => new[]
        //{
        //    new Alex()
        //};
        //
        //public override DrawableVitaruPlayer GetDrawablePlayer(VitaruPlayfield playfield, VitaruPlayer player)
        //{
        //    switch (player.Name)
        //    {
        //        default:
        //            return null;
        //        case "Alex":
        //            return new DrawableVitaruPlayer(playfield, player);
        //    }
        //}
    }
}