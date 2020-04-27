// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Vitaru.Chapters;
using Vitaru.Gamemodes.Vitaru.HitObjects;
using Vitaru.Gamemodes.Vitaru.HitObjects.DrawableHitObjects;

#endregion

namespace Vitaru.Gamemodes.Vitaru
{
    public class VitaruChapterSet : ChapterSet
    {
        public override string Name => "Vitaru";

        public override string Description =>
            "The movement gamemode, Vitaru is all about moving out of the way to the beat.";

        public override Chapter[] GetChapters() => new Chapter[]
        {
            new RejectChapter()
        };

        public override Cluster GetCluster() => new VitaruCluster();

        public override DrawableCluster GetDrawableCluster(Cluster cluster, VitaruPlayfield playfield) =>
            new DrawableVitaruCluster((VitaruCluster) cluster, playfield);

        public override Bullet GetBullet() => new Bullet();

        public override DrawableBullet GetDrawableBullet(Bullet bullet, VitaruPlayfield playfield) =>
            new DrawableBullet(bullet, playfield);

        public override Laser GetLaser() => new Laser();

        public override DrawableLaser GetDrawableLaser(Laser laser, VitaruPlayfield playfield) =>
            new DrawableLaser(laser, playfield);

        public override Enemy GetEnemy(VitaruPlayfield playfield, DrawableCluster drawableCluster) =>
            new Enemy(playfield, (DrawableVitaruCluster) drawableCluster);
    }
}