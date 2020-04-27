// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Gamemodes
{
    public abstract class ChapterSet
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} ChapterSet.";

        public abstract Chapter[] GetChapters();

        public virtual float PlayfieldMargin => 0.8f;

        public virtual Vector2 PlayfieldAspectRatio => new Vector2(5, 4);

        public virtual Vector2 PlayfieldSize => new Vector2(1024, 820);

        public virtual Vector4 PlayfieldBounds => new Vector4(0, 0, PlayfieldSize.X, PlayfieldSize.Y);

        public virtual Vector2 PlayerStartingPosition => new Vector2(PlayfieldSize.X / 2, 700);

        public virtual Vector2 ClusterOffset => new Vector2(256, 0);

        //TODO: Move below to an object converter?

        public abstract Cluster GetCluster();

        public abstract DrawableCluster GetDrawableCluster(Cluster cluster, VitaruPlayfield playfield);

        public abstract Bullet GetBullet();

        public abstract DrawableBullet GetDrawableBullet(Bullet bullet, VitaruPlayfield playfield);

        public abstract Laser GetLaser();

        public abstract DrawableLaser GetDrawableLaser(Laser laser, VitaruPlayfield playfield);

        public abstract Enemy GetEnemy(VitaruPlayfield playfield, DrawableCluster drawablePattern);
    }
}