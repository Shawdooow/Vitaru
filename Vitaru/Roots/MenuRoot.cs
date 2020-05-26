// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Vitaru.Graphics;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public abstract class MenuRoot : ExitableRoot
    {
        protected virtual bool UseLevelBackground => false;

        protected readonly ShadeLayer<IDrawable2D> ShadeLayer;
        protected readonly Sprite Background;
        protected readonly Box Dim;

        protected MenuRoot()
        {
            Add(ShadeLayer = new ShadeLayer<IDrawable2D>
            {
                Children = new[]
                {
                    Background = new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    Dim = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Size = new Vector2(512),
                        Scale = new Vector2(4),
                    }
                }
            });

            if (UseLevelBackground && TrackManager.CurrentTrack.Level.Image != string.Empty)
                Background.Texture =
                    Vitaru.LevelTextureStore.GetTexture(
                        $"{TrackManager.CurrentTrack.Level.Name}\\{TrackManager.CurrentTrack.Level.Image}");
        }
    }
}