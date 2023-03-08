﻿// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using System.Numerics;

namespace Vitaru.Play
{
    //only the UI for the gamefield
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