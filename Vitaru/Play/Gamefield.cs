// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Play
{
    public class Gamefield
    {
        public static double Current { get; private set; } = double.MinValue;

        public virtual Shades Shade { get; set; }

        public virtual float Intensity { get; set; } = 1;

        public Gamefield()
        {

        }

        public class GamefieldBorder : Layer2D<Box>
        {
            public GamefieldBorder(Vector2 size)
            {
                Size = size;

                const int w = 2;
                Children = new[]
                {
                    new Box
                    {
                        Height = w,
                        Width = size.X + w,
                        Y = -size.Y / 2,
                    },
                    new Box
                    {
                        Height = w,
                        Width = size.X + w,
                        Y = size.Y / 2,
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y + w,
                        X = -size.X / 2,
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y + w,
                        X = size.X / 2,
                    },
                };
            }
        }
    }
}