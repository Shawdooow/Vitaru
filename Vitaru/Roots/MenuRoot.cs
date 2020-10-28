// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Graphics.Overlays;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public abstract class MenuRoot : ExitableRoot
    {
        public override string Name => nameof(MenuRoot);

        protected virtual bool UseLevelBackground => false;

        protected virtual bool Parallax => false;

        protected virtual float ParallaxAmount => 10;

        protected virtual bool InvertParallax => false;

        protected readonly Layer2D<IDrawable2D> BackgroundLayer;
        protected readonly Sprite Background;
        protected readonly Box Dim;
        protected Button Back;

        private string bg = string.Empty;

        protected MenuRoot()
        {
            Add(BackgroundLayer = new Layer2D<IDrawable2D>
            {
                Children = new[]
                {
                    Background = new Sprite(ThemeManager.GetBackground())
                    {
                        Name = "Background",
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    },
                    Dim = new Box
                    {
                        Name = "Dim",
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Size = new Vector2(Renderer.Width, Renderer.Height),
                        AutoScaleDirection = Direction.Both
                    }
                }
            });

            if (Parallax)
            {
                Background.Width += ParallaxAmount;
                Background.Height += ParallaxAmount;
            }

            if (UseLevelBackground && TrackManager.CurrentTrack.Level.Image != string.Empty)
                Background.Texture =
                    Vitaru.LevelTextureStore.GetTexture(
                        $"{TrackManager.CurrentTrack.Level.Title}\\{TrackManager.CurrentTrack.Level.Image}");
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            TrackManager.OnTrackChange += TrackChange;

            if (Back == null)
                Add(Back = new Button
                {
                    Name = "Back",
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
                    SpriteText =
                    {
                        TextScale = 0.35f
                    },

                    OnClick = DropRoot
                });
            Add(new PerformanceDisplay(DisplayType.FPS));
            Add(Cursor = new Cursor());
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);

            Background.Size = Parallax ? new Vector2(size.X + ParallaxAmount, size.Y + ParallaxAmount) : size;
            Dim.Size = size;
        }

        public override void Update()
        {
            base.Update();

            if (Parallax)
            {
                Vector2 min = new Vector2(-ParallaxAmount / 2);
                Vector2 max = new Vector2(ParallaxAmount / 2);

                Vector2 parallax = PrionMath.Scale(InputManager.Mouse.Position,
                    new Vector2(Renderer.Width / -2f, Renderer.Height / -2f),
                    new Vector2(Renderer.Width / 2f, Renderer.Height / 2f), min, max);

                if (InvertParallax)
                    parallax *= -1;

                Background.Position = PrionMath.Clamp(parallax, min, max);
            }
        }

        public override void PreRender()
        {
            base.PreRender();

            if (bg != string.Empty && UseLevelBackground)
            {
                Background.Texture =
                    bg == "default" ? ThemeManager.GetBackground() : Vitaru.LevelTextureStore.GetTexture(bg);
                bg = string.Empty;
            }
        }

        protected virtual void TrackChange(Track t)
        {
            if (t.Level.Image != string.Empty)
                bg = $"{t.Level.Title}\\{t.Level.Image}";
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