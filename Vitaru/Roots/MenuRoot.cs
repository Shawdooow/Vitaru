// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.UserInterface;
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
        protected readonly Button Back;

        protected MenuRoot()
        {
            Add(ShadeLayer = new ShadeLayer<IDrawable2D>
            {
                Children = new[]
                {
                    Background = new Sprite(Vitaru.GetBackground())
                    {
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    },
                    Dim = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    }
                }
            });

            Add(Back = new Button
            {
                ParentOrigin = Mounts.BottomLeft,
                Origin = Mounts.BottomLeft,
                Position = new Vector2(10, -10),
                Size = new Vector2(80, 40),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.Red
                },

                Text = "Back",

                OnClick = DropRoot
            });

            if (UseLevelBackground && TrackManager.CurrentTrack.Level.Image != string.Empty)
                Background.Texture =
                    Vitaru.LevelTextureStore.GetTexture(
                        $"{TrackManager.CurrentTrack.Level.Name}\\{TrackManager.CurrentTrack.Level.Image}");
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            Background.Size = size;
            Dim.Size = size;
        }
    }
}