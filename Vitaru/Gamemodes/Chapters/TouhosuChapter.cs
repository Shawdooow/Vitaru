// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

namespace Vitaru.Gamemodes.Chapters
{
    public class TouhosuChapter : Chapter
    {
        public override string Title => "Touhosu";

        public sealed override VitaruPlayer[] GetPlayers() => GetTouhosuPlayers();

        public sealed override DrawableVitaruPlayer GetDrawablePlayer(VitaruPlayfield playfield, VitaruPlayer player) =>
            GetDrawableTouhosuPlayer(playfield, (TouhosuPlayer) player);

        public virtual TouhosuPlayer[] GetTouhosuPlayers() => null;

        public virtual DrawableTouhosuPlayer
            GetDrawableTouhosuPlayer(VitaruPlayfield playfield, TouhosuPlayer player) => null;
    }
}