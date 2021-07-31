// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Debug;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Chapters
{
    public class TauChapter : Chapter
    {
        public override string Title => "Tau";

        public override Player[] GetPlayers(Gamefield gamefield = null)
        {
            throw Debugger.NotImplemented("");
        }

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