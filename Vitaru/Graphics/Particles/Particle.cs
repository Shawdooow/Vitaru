// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing


using System.Numerics;

namespace Vitaru.Graphics.Particles
{
    public struct Particle
    {
        public Vector4 StartPosition;

        public Vector4 EndPosition;

        public Vector4 Color;

#pragma warning disable 169
        /// <summary>
        /// Only here so when we buffer to the gpu we have the correct alignment.
        /// This is filled out by the compute shader for the vertex shader to read
        /// </summary>
        private Matrix4x4 model;
#pragma warning restore 169
    }
}