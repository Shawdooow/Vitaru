// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities;
using Vitaru.Roots;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Mods.Included
{
    public class Mixer : Mod
    {
        public override Button GetMenuButton() =>
            new Button
            {
                Y = -180,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta
                },

                Text = "Mixer"
            };

        public override Root GetRoot() => new MixerRoot();

        private class MixerRoot : MenuRoot
        {
            public override string Name => nameof(MixerRoot);

            protected override bool UseLevelBackground => true;

            protected override bool Parallax => true;

            private const float min = 0.05f;
            private const float max = 2f;

            private float rate = 1f;

            private TrackController controller;

            private Slider slider;
            private SpriteText pitch;

            public override void LoadingComplete()
            {
                AddArray(new ILayer[]
                {
                    new Button
                    {
                        Position = new Vector2(360, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.25f
                        },

                        Text = "+ 0.25x",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.25f)
                    },
                    new Button
                    {
                        Position = new Vector2(240, 0),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.5f
                        },

                        Text = "1.5x",
                        OnClick = () => setRate(1.5f)
                    },
                    new Button
                    {
                        Position = new Vector2(120, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.25f
                        },

                        Text = "+ 0.05x",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.05f)
                    },
                    new Button
                    {
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.5f
                        },

                        Text = "1x",
                        OnClick = () => setRate(1f)
                    },
                    new Button
                    {
                        Position = new Vector2(-120, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.25f
                        },

                        Text = "- 0.05x",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.05f)
                    },
                    new Button
                    {
                        Position = new Vector2(-240, 0),
                        Size = new Vector2(100, 100),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.PrimaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.5f
                        },

                        Text = "0.75x",
                        OnClick = () => setRate(0.75f)
                    },
                    new Button
                    {
                        Position = new Vector2(-360, 0),
                        Size = new Vector2(80, 80),

                        Background = Game.TextureStore.GetTexture("square.png"),
                        Dim =
                        {
                            Alpha = 0.5f
                        },
                        BackgroundSprite =
                        {
                            Color = ThemeManager.SecondaryColor
                        },
                        SpriteText =
                        {
                            TextScale = 0.25f
                        },
                        Text = "- 0.25x",
                        OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.25f)
                    },

                    slider = new Slider
                    {
                        Position = new Vector2(0, 100),
                        OnProgressInput = p => setRate(PrionMath.Scale(p, 0, 1, min, max))
                    },
                    pitch = new SpriteText
                    {
                        Position = new Vector2(0, -200),
                        Text = TrackManager.CurrentTrack.Pitch.ToString()
                    },
                    controller = new TrackController()
                });

                setRate(TrackManager.CurrentTrack.Pitch);

                base.LoadingComplete();
            }

            public override void Update()
            {
                base.Update();

                controller.Update();
                controller.TryRepeat();
            }

            protected override void TrackChange(Track t)
            {
                base.TrackChange(t);
                t.Pitch = rate;
            }

            private void setRate(float r)
            {
                TrackManager.CurrentTrack.Pitch = rate = Math.Clamp(r, min, max);
                pitch.Text = MathF.Round(r, 2).ToString();
                slider.Progress = PrionMath.Scale(rate, min, max);
            }
        }
    }
}