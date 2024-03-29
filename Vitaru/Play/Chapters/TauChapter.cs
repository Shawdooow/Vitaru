﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Debug;
using Vitaru.Chapters;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Play.Chapters
{
    public class TauChapter : Chapter
    {
        public override string Title => "Tau";

        public override Player[] GetPlayers(Gamefield gamefield = null) => throw Debugger.NotImplemented("");

        //public sealed override VitaruPlayer[] GetPlayers() => GetTouhosuPlayers();
        //
        //public sealed override DrawableVitaruPlayer GetDrawablePlayer(VitaruPlayfield playfield, VitaruPlayer player) =>
        //    GetDrawableTouhosuPlayer(playfield, (TouhosuPlayer) player);
        //
        //public virtual TouhosuPlayer[] GetTouhosuPlayers() => null;
        //
        //public virtual DrawableTouhosuPlayer
        //   GetDrawableTouhosuPlayer(VitaruPlayfield playfield, TouhosuPlayer player) => null;
    }
}