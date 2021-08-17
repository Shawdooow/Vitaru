// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Input.Receivers;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Time;

namespace Vitaru.Editor.UI
{
    public class Timeline : InputLayer<IDrawable2D>, IHasInputKeys
    {
        private const float width = 1280f;
        private const float height = 100f;

        private readonly Text2D timeIn;
        private readonly Text2D msIn;
        private readonly Slider scrubber;
        private readonly Text2D timeLeft;
        private readonly Text2D msLeft;

        private readonly Text2D percentGain;

        private readonly Button play;

        private readonly Slider pitch;
        private readonly Slider gain;

        private EditableStartTime start;

        public Timeline(LevelManager manager)
        {
            manager.PropertiesSet += Selected;

            ParentOrigin = Mounts.TopCenter;
            Origin = Mounts.TopCenter;

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
                    Width = width - 280,

                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    OnProgressInput = p =>
                        TrackManager.CurrentTrack.Seek(PrionMath.Remap(p, 0, 1, 0,
                            TrackManager.CurrentTrack.Sample.Length))
                },
                play = new Button
                {
                    Y = -8,
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,
                    Size = new Vector2(48),
                    Background = Vitaru.TextureStore.GetTexture("pause.png"),

                    OnClick = TogglePlay
                },
                pitch = new Slider
                {
                    Width = 200,
                    Position = new Vector2(80, -8),
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,

                    OnProgressInput = p => TrackManager.CurrentTrack.Pitch = PrionMath.Remap(p, 0, 1, 0.5f, 1.5f)
                },

                gain = new Slider
                {
                    Width = 200,
                    Position = new Vector2(-80, -8),
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,

                    OnProgressInput = p => TrackManager.CurrentTrack.Gain = p
                }
            };

            scrubber.AddArray(new IDrawable2D[]
            {
                timeIn = new Text2D
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.BottomRight,
                    Position = new Vector2(-12, -2),
                    FontScale = 0.25f
                },
                msIn = new Text2D
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.TopRight,
                    Position = new Vector2(-12, 2),
                    FontScale = 0.25f
                },
                timeLeft = new Text2D
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.BottomLeft,
                    Position = new Vector2(12, -2),
                    FontScale = 0.25f
                },
                msLeft = new Text2D
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.TopLeft,
                    Position = new Vector2(12, 2),
                    FontScale = 0.25f
                }
            });

            pitch.AddArray(new IDrawable2D[]
            {
                new Button
                {
                    Size = new Vector2(48, 24),
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    X = -12,
                    Text2D =
                    {
                        FontScale = 0.25f,
                        Text = "0.5x"
                    },

                    OnClick = () =>
                    {
                        pitch.Progress = 0f;
                        TrackManager.CurrentTrack.Pitch = 0.5f;
                    }
                },
                new Button
                {
                    Size = new Vector2(32, 20),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.BottomCenter,
                    Y = -12,
                    Text2D =
                    {
                        FontScale = 0.25f,
                        Text = "1x"
                    },

                    OnClick = () =>
                    {
                        pitch.Progress = 0.5f;
                        TrackManager.CurrentTrack.Pitch = 1f;
                    }
                },
                new Button
                {
                    Size = new Vector2(48, 24),
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    X = 12,
                    Text2D =
                    {
                        FontScale = 0.25f,
                        Text = "1.5x"
                    },

                    OnClick = () =>
                    {
                        pitch.Progress = 1f;
                        TrackManager.CurrentTrack.Pitch = 1.5f;
                    }
                }
            });

            gain.Add(percentGain = new Text2D
            {
                ParentOrigin = Mounts.TopCenter,
                Origin = Mounts.BottomCenter,
                Y = -12,

                FontScale = 0.25f,
                Text = "1x"
            });

            pitch.Progress = 0.5f;
            gain.Progress = TrackManager.CurrentTrack.Gain;
        }

        public void Selected(EditableProperty[] properties)
        {
            start = null;

            if (properties != null)
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

            if (TrackManager.CurrentTrack.Playing && TrackManager.CurrentTrack.CheckFinish())
            {
                TogglePlay();
                TrackManager.CurrentTrack.Seek(TrackManager.CurrentTrack.Sample.Length);
            }

            if (TrackManager.CurrentTrack.Playing)
                start?.SetValue(Math.Round(TrackManager.CurrentTrack.Clock.Current, 2));

            float current = (float) TrackManager.CurrentTrack.Clock.Current;
            float length = (float) TrackManager.CurrentTrack.Sample.Length * 1000;

            if (!scrubber.Dragging)
                scrubber.Progress = PrionMath.Remap(current, 0, length);

            TimeSpan t = TimeSpan.FromMilliseconds(current);
            TimeSpan l = TimeSpan.FromMilliseconds(length - current);

            string time = $"{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
            string left = $"-{l.Minutes:D2}:{l.Seconds:D2}:{l.Milliseconds:D3}";

            timeIn.Text = time;
            msIn.Text = Math.Round(t.TotalMilliseconds, 2).ToString();
            timeLeft.Text = left;
            msLeft.Text = $"-{Math.Round(l.TotalMilliseconds, 2)}";

            percentGain.Text = $"{MathF.Round(TrackManager.CurrentTrack.Gain * 100, 0)}%";
        }

        public bool OnKeyDown(KeyboardKeyEvent e)
        {
            if (Renderer.Window.Focused && e.Key == Keys.Space)
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