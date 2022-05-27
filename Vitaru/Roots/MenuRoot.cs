// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Graphics.Overlays;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Graphics.UI;

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
        protected VitaruButton Back;

        private string bg = string.Empty;
        private bool ft = true;

        protected MenuRoot()
        {
            Add(BackgroundLayer = new Layer2D<IDrawable2D>
            {
                Children = new[]
                {
                    Background = new Sprite(ThemeManager.GetBackground())
                    {
                        Name = "Background",
                        Size = new Vector2(Renderer.Size.X, Renderer.Size.Y),
                        AutoScaleDirection = Direction.Both,
                    },
                    Dim = new Box
                    {
                        Name = "Dim",
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Size = new Vector2(Renderer.Size.X, Renderer.Size.Y),
                        AutoScaleDirection = Direction.Both,
                    },
                },
            });

            if (Parallax)
            {
                Background.Width += ParallaxAmount;
                Background.Height += ParallaxAmount;
            }

            if (UseLevelBackground && TrackManager.CurrentTrack.Metadata.Image != string.Empty)
                Background.Texture = TrackManager.CurrentTrack.Metadata.Image[0] == '#' ? 
                    Vitaru.TextureStore.GetTexture(
                        TrackManager.CurrentTrack.Metadata.Image.Trim('#'), TrackManager.CurrentTrack.Metadata.Filtering) : 
                    Vitaru.LevelTextureStore.GetTexture(
                        $"{TrackManager.CurrentTrack.Metadata.Title}\\{TrackManager.CurrentTrack.Metadata.Image}",
                        TrackManager.CurrentTrack.Metadata.Filtering);
        }

        public override void RenderingLoadingComplete()
        {
            base.RenderingLoadingComplete();
            TrackManager.OnTrackChange += TrackChange;

            if (Back == null)
                Add(Back = new VitaruButton
                {
                    Name = "Back",
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,
                    Position = new Vector2(10, -10),
                    Size = new Vector2(80, 40),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Color = Color.Red,

                    Text = "Back",
                    Text2D =
                    {
                        FontScale = 0.35f,
                    },

                    OnClick = DropRoot,
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
                Vector2 min = new(-ParallaxAmount / 2);
                Vector2 max = new(ParallaxAmount / 2);

                Vector2 parallax = PrionMath.Remap(InputManager.Mouse.Position,
                    new Vector2(Renderer.Size.X / -2f, Renderer.Size.Y / -2f),
                    new Vector2(Renderer.Size.X / 2f, Renderer.Size.Y / 2f), min, max);

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
                if (bg[0] == '#')
                    Background.Texture = Vitaru.TextureStore.GetTexture(bg.Trim('#'), ft);
                else
                    Background.Texture =
                        bg == "default" ? ThemeManager.GetBackground() : Vitaru.LevelTextureStore.GetTexture(bg, ft);
                bg = string.Empty;
                ft = true;
            }
        }

        protected virtual void TrackChange(Track t)
        {
            if (t.Metadata.Image != string.Empty)
            {
                bg = t.Metadata.Image[0] == '#' ? t.Metadata.Image : $"{t.Metadata.Title}\\{t.Metadata.Image}";
                ft = t.Metadata.Filtering;
            }
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