// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Vitaru.Editor.KeyFrames;

namespace Vitaru.Play.Projectiles
{
    public abstract class Projectile : GameEntity, IHasKeyFrames
    {
        public override string Name { get; set; } = nameof(Projectile);

        public List<KeyValuePair<int, List<KeyFrame>>> KeyFrames { get; set; } = new();

        protected Random Random;

        public virtual void ConcurrentUpdate(Random random)
        {
            Random = random;
            Update();
        }
    }
}