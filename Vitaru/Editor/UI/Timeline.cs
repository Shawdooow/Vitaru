// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities;
using Vitaru.Tracks;

namespace Vitaru.Editor.UI
{
    public class Timeline : InputLayer<IDrawable2D>
    {
        private const float width = 1080f;
        private const float height = 140f;

        private readonly SpriteText timeIn;
        private readonly Slider scrubber;
        private readonly SpriteText timeLeft;

        public Timeline()
        {
            ParentOrigin = Mounts.BottomCenter;
            Origin = Mounts.BottomCenter;

            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new SpriteLayer
                {
                    Name = "Background",
                    Child = new Box
                    {
                        Alpha = 0.8f,
                        Size = new Vector2(width, height),
                        Color = Color.Black
                    }
                },
                scrubber = new Slider
                {
                    OnProgressInput = p =>
                        TrackManager.CurrentTrack.Seek(PrionMath.Scale(p, 0, 1, 0, TrackManager.CurrentTrack.Length))
                }
            };

            scrubber.AddArray(new []
            {
                timeIn = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    X = -12,
                    TextScale = 0.25f,
                },
                timeLeft = new SpriteText
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    X = 12,
                    TextScale = 0.25f,
                }
            });
        }

        public void Update()
        {
            float current = (float)TrackManager.CurrentTrack.Clock.Current;
            float length = (float)TrackManager.CurrentTrack.Length * 1000;

            if (!scrubber.Dragging)
                scrubber.Progress = PrionMath.Scale(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            timeLeft.Text = left;
        }
    }
}