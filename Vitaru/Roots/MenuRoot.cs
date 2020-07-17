// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UserInterface;
using Vitaru.Graphics;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public abstract class MenuRoot : ExitableRoot
    {
        public override string Name => nameof(MenuRoot);

        protected virtual bool UseLevelBackground => false;

        protected readonly ShadeLayer<IDrawable2D> ShadeLayer;
        protected readonly Sprite Background;
        protected readonly Box Dim;
        protected readonly Button Back;

        private string bg = string.Empty;

        protected MenuRoot()
        {
            Add(ShadeLayer = new ShadeLayer<IDrawable2D>
            {
                Children = new[]
                {
                    Background = new Sprite(ThemeManager.GetBackground())
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

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            TrackManager.OnTrackChange += TrackChange;
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            Background.Size = size;
            Dim.Size = size;
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty && UseLevelBackground)
            {
                Background.Texture =
                    bg == "default" ? ThemeManager.GetBackground() : Vitaru.LevelTextureStore.GetTexture(bg);
                bg = string.Empty;

                //TODO: hack to force an update
                Background.Size = Background.Size;
            }
        }

        protected virtual void TrackChange(Track t)
        {
            if (t.Level.Image != string.Empty)
                bg = $"{t.Level.Name}\\{t.Level.Image}";
            else
                bg = "default";
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            TrackManager.OnTrackChange -= TrackChange;
        }
    }
}