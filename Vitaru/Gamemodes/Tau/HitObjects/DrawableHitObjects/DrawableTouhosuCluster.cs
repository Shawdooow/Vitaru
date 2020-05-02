// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Vitaru.HitObjects.DrawableHitObjects;

#endregion

namespace Vitaru.Gamemodes.Tau.HitObjects.DrawableHitObjects
{
    public class DrawableTouhosuCluster : DrawableVitaruCluster
    {
        public DrawableTouhosuCluster(TouhosuCluster cluster)
            : base(cluster)
        {
        }

        public DrawableTouhosuCluster(TouhosuCluster cluster, VitaruPlayfield playfield)
            : base(cluster, playfield)
        {
        }
    }
}