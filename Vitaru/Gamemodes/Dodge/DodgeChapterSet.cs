// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Chapters;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki;

namespace Vitaru.Gamemodes.Dodge
{
    public class DodgeChapterSet : Chapterset
    {
        public override string Name => "Dodge";

        public override string Description =>
            "The simple dodging experience, no shooting, no killing, no place to hide.";

        public override Vector2 PlayfieldSize => new(512 + 256, 384);

        public override Vector2 PlayerStartingPosition => PlayfieldSize / 2;

        public override Chapter[] GetChapters() => new Chapter[]
        {
            new AlkiChapter()
        };

        //public override Cluster GetCluster() => new VitaruCluster();
        //
        //public override DrawableCluster GetDrawableCluster(Cluster cluster, VitaruPlayfield playfield) =>
        //    new DrawableVitaruCluster((VitaruCluster) cluster, playfield);
        //
        //public override Bullet GetBullet() => new Bullet();
        //
        //public override DrawableBullet GetDrawableBullet(Bullet bullet, VitaruPlayfield playfield) =>
        //    new DrawableBullet(bullet, playfield);
        //
        //public override Laser GetLaser() => new Laser();
        //
        //public override DrawableLaser GetDrawableLaser(Laser laser, VitaruPlayfield playfield) =>
        //    new DrawableLaser(laser, playfield);
        //
        //public override Enemy GetEnemy(VitaruPlayfield playfield, DrawableCluster drawablePattern) =>
        //    new Enemy(playfield, (DrawableVitaruCluster) drawablePattern);
    }
}