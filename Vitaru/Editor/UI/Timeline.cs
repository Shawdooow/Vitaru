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
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Input.Receivers;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Time;
using Vitaru.Tracks;

namespace Vitaru.Editor.UI
{
    public class Timeline : InputLayer<IDrawable2D>, IHasInputKeys
    {
        private const float width = 1080f;
        private const float height = 140f;

        private readonly SpriteText timeIn;
        private readonly SpriteText msIn;
        private readonly Slider scrubber;
        private readonly SpriteText timeLeft;
        private readonly SpriteText msLeft;

        private readonly Button play;

        private readonly Slider speed;

        private EditableStartTime start;

        public Timeline(LevelManager manager)
        {
            manager.PropertiesSet += Selected;

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
                    Y = 8,
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    OnProgressInput = p =>
                        TrackManager.CurrentTrack.Seek(PrionMath.Scale(p, 0, 1, 0, TrackManager.CurrentTrack.Length))
                },
                play = new Button
                {
                    Position = new Vector2(0, -12),
                    ParentOrigin = Mounts.Center,
                    Origin = Mounts.Center,
                    Size = new Vector2(32),
                    Background = Vitaru.TextureStore.GetTexture("pause.png"),

                    OnClick = TogglePlay
                },
                speed = new Slider
                {
                    Width = 200,
                    Y = -8,
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,

                    OnProgressInput = p => TrackManager.CurrentTrack.Pitch = PrionMath.Scale(p, 0, 1, 0.5f, 1.5f)
                }
            };

            scrubber.AddArray(new IDrawable2D[]
            {
                timeIn = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.BottomRight,
                    Position = new Vector2(-12, -2),
                    TextScale = 0.25f
                },
                msIn = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.TopRight,
                    Position = new Vector2(-12, 2),
                    TextScale = 0.25f
                },
                timeLeft = new SpriteText
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.BottomLeft,
                    Position = new Vector2(12, -2),
                    TextScale = 0.25f
                },
                msLeft = new SpriteText
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.TopLeft,
                    Position = new Vector2(12, 2),
                    TextScale = 0.25f
                }
            });

            speed.AddArray(new IDrawable2D[]
            {
                new Button
                {
                    Size = new Vector2(48, 24),
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    X = -12,
                    SpriteText =
                    {
                        TextScale = 0.25f,
                        Text = "0.5x"
                    },

                    OnClick = () =>
                    {
                        speed.Progress = 0f;
                        TrackManager.CurrentTrack.Pitch = 0.5f;
                    }
                },
                new Button
                {
                    Size = new Vector2(32, 20),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.BottomCenter,
                    Y = -12,
                    SpriteText =
                    {
                        TextScale = 0.25f,
                        Text = "1x"
                    },

                    OnClick = () =>
                    {
                        speed.Progress = 0.5f;
                        TrackManager.CurrentTrack.Pitch = 1f;
                    }
                },
                new Button
                {
                    Size = new Vector2(48, 24),
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    X = 12,
                    SpriteText =
                    {
                        TextScale = 0.25f,
                        Text = "1.5x"
                    },

                    OnClick = () =>
                    {
                        speed.Progress = 1f;
                        TrackManager.CurrentTrack.Pitch = 1.5f;
                    }
                }
            });

            speed.Progress = 0.5f;
        }

        public void Selected(EditableProperty[] properties)
        {
            start = null;
            for (int i = 0; i < properties.Length; i++)
                if (properties[i] is EditableStartTime s)
                {
                    start = s;
                    break;
                }
        }

        public void TogglePlay()
        {
            if (TrackManager.CurrentTrack.Playing)
            {
                TrackManager.CurrentTrack.Pause();
                play.Background = Vitaru.TextureStore.GetTexture("play.png");
            }
            else
            {
                TrackManager.CurrentTrack.Play();
                play.Background = Vitaru.TextureStore.GetTexture("pause.png");
            }
        }

        public void Update()
        {
            TrackManager.CurrentTrack.Clock.Update();

            if (TrackManager.CurrentTrack.Playing)
                start?.SetValue(Math.Round(TrackManager.CurrentTrack.Clock.Current, 2));

            float current = (float) TrackManager.CurrentTrack.Clock.Current;
            float length = (float) TrackManager.CurrentTrack.Length * 1000;

            if (!scrubber.Dragging)
                scrubber.Progress = PrionMath.Scale(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            msIn.Text = Math.Round(t.TotalMilliseconds, 2).ToString();
            timeLeft.Text = left;
            msLeft.Text = $"-{Math.Round(l.TotalMilliseconds, 2)}";
        }

        public bool OnKeyDown(KeyboardKeyEvent e)
        {
            if (e.Key == Keys.Space)
            {
                TogglePlay();
                return true;
            }

            return false;
        }

        public bool OnKeyUp(KeyboardKeyEvent e)
        {
            return true;
        }
    }
}