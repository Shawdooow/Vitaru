// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Chapters;
using Vitaru.Gamemodes.Tau.Chapters.Rational;
using Vitaru.Gamemodes.Tau.Chapters.Scarlet;
using Vitaru.Gamemodes.Tau.Chapters.Worship;
using Vitaru.Gamemodes.Tau.HitObjects;
using Vitaru.Gamemodes.Tau.HitObjects.DrawableHitObjects;

#endregion

namespace Vitaru.Gamemodes.Tau
{
    public class TouhosuChapterSet : ChapterSet
    {
        public override string Name => "Touhosu";

        public override string Description =>
            "The original bullet dodging experiance, bullets are sent your way to the beat.";

        public override Chapter[] GetChapters() => new TouhosuChapter[]
        {
            new WorshipChapter(),
            new ScarletChapter(),
            new RationalChapter()
        };

        public override Cluster GetCluster() => new TouhosuCluster();

        public override DrawableCluster GetDrawableCluster(Cluster cluster, VitaruPlayfield playfield) =>
            new DrawableTouhosuCluster((TouhosuCluster) cluster, playfield);

        public override Bullet GetBullet() => new Bullet();

        public override DrawableBullet GetDrawableBullet(Bullet bullet, VitaruPlayfield playfield) =>
            new DrawableBullet(bullet, playfield);

        public override Laser GetLaser() => new Laser();

        public override DrawableLaser GetDrawableLaser(Laser laser, VitaruPlayfield playfield) =>
            new DrawableLaser(laser, playfield);

        public override Enemy GetEnemy(VitaruPlayfield playfield, DrawableCluster drawablePattern) =>
            new Enemy(playfield, (DrawableTouhosuCluster) drawablePattern);
    }
}