// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UserInterface;
using Vitaru.Roots;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Mods
{
    public class LevelMixer : Mod
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

        public override Root GetRoot() => new Mixer();

        private class Mixer : MenuRoot
        {
            public override string Name => nameof(Mixer);

            protected override bool UseLevelBackground => true;

            private SpriteText pitch;

            public override void LoadingComplete()
            {
                Add(new Button
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
                });

                Add(new Button
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
                });

                Add(new Button
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
                });

                Add(new Button
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
                });

                Add(new Button
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
                });

                Add(pitch = new SpriteText
                {
                    Position = new Vector2(0, -200),
                    Text = TrackManager.CurrentTrack.Pitch.ToString()
                });

                base.LoadingComplete();
            }

            private void setRate(float rate)
            {
                TrackManager.CurrentTrack.Pitch = Math.Max(0.05f, MathF.Round(rate, 2));
                pitch.Text = TrackManager.CurrentTrack.Pitch.ToString();
            }
        }
    }
}