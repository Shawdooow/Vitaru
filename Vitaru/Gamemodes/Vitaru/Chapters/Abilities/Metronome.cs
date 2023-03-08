// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using System.Drawing;
using System.Numerics;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Abilities
{
    public class Metronome : Layer2D<IDrawable2D>
    {
        private readonly Box marker;

        public Metronome(Player player, Layer2D<IDrawable2D> overlays)
        {
            ParentOrigin = Mounts.TopCenter;
            Origin = Mounts.BottomCenter;

            Y = -10;

            Size = new Vector2(200, 60);

            const float w = 16;
            float spacing = Width / 4;

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Color = Color.Black,
                    Alpha = 0.8f,
                    Size = Size,
                },
                new Box
                {
                    Size = new Vector2(w, Height),
                    X = spacing,
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.Center,
                    Color = player.ComplementaryColor
                },
                new Box
                {
                    Size = new Vector2(w, Height),
                    X = spacing * 2,
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.Center,
                    Color = player.SecondaryColor
                },
                new Box
                {
                    Size = new Vector2(w, Height),
                    X = spacing * 3,
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.Center,
                    Color = player.ComplementaryColor
                },
                new Box
                {
                    Size = new Vector2(w, Height),
                    X = spacing * 4,
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.Center,
                    Color = player.PrimaryColor
                },
                marker = new Box
                {
                    Size = new Vector2(w / 2, Height),
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.Center,
                },
            };
        }
    }
}
