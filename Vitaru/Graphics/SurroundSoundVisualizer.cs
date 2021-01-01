// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Utilities;
using Vitaru.Themes;
using Vitaru.Tracks;

namespace Vitaru.Graphics
{
    public class SurroundSoundVisualizer : Layer2D<IDrawable2D>
    {
        private readonly Bumper left;
        private readonly Bumper right;

        public SurroundSoundVisualizer()
        {
            Children = new[]
            {
                left = new Bumper
                {
                    Position = TrackManager.CurrentTrack.Source.LeftPosition.XZ(),
                    Circle =
                    {
                        Color = Color.Blue
                    }
                },
                right = new Bumper
                {
                    Position = TrackManager.CurrentTrack.Source.RightPosition.XZ(),
                    Circle =
                    {
                        Color = Color.Red
                    }
                }
            };
        }

        public void OnNewBeat()
        {
            left.Pulse();
            right.Pulse();
        }

        public class Bumper : Layer2D<IDrawable2D>
        {
            private readonly Sprite glow1;
            private readonly Sprite glow2;
            public readonly Sprite Circle;

            public Bumper()
            {
                Children = new IDrawable2D[]
                {
                    glow1 = new Sprite
                    {
                        Texture = Game.TextureStore.GetTexture("Cursor\\glow.png"),
                        Size = new Vector2(128),
                        Scale = new Vector2(0.5f),
                        Alpha = 0.5f,
                        Color = ThemeManager.SecondaryColor
                    },
                    glow2 = new Sprite
                    {
                        Texture = Game.TextureStore.GetTexture("Cursor\\glow.png"),
                        Size = new Vector2(128),
                        Scale = new Vector2(0.5f),
                        Alpha = 0,
                        Color = ThemeManager.SecondaryColor
                    },
                    Circle = new Sprite
                    {
                        Texture = Game.TextureStore.GetTexture("Cursor\\ring.png"),
                        Size = new Vector2(128),
                        Scale = new Vector2(0.5f)
                    }
                };
            }

            public void Pulse()
            {
                if (glow2.Alpha <= 0)
                    pulse(glow1, glow2);
                else
                    pulse(glow2, glow1);
            }

            private void pulse(Sprite current, Sprite flip)
            {
                double length = TrackManager.CurrentTrack.Level.GetBeatLength() * 0.9f;

                current.ScaleTo(new Vector2(0.75f), length, Easings.OutCubic);
                current.FadeTo(0, length, Easings.InCubic);

                flip.Scale = new Vector2(0.5f);
                flip.FadeTo(0.5f, length, Easings.InCubic);

                Circle.ScaleTo(new Vector2(0.45f), 20, Easings.OutQuart).OnComplete(() =>
                    Circle.ScaleTo(new Vector2(0.5f), length - 20, Easings.OutSine));
            }
        }
    }
}