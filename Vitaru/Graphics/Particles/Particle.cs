// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Vitaru.Graphics.Particles
{
    public struct Particle
    {
        public Vector2 StartPosition { get; init; }

        public Vector2 EndPosition { get; init; }

        public Vector3 Color { get; init; }

        public float Scale { get; init; }
    }
}