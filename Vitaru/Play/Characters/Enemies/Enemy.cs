// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Editor.KeyFrames;
using Vitaru.Editor.KeyFrames.Interfaces;

namespace Vitaru.Play.Characters.Enemies
{
    public class Enemy : Character, IHasKeyFrames, IHasPosition, IHasAlpha
    {
        public List<KeyValuePair<int, List<KeyFrame>>> KeyFrames { get; set; }

        public Enemy(Gamefield gamefield) : base(gamefield) { }
    }
}