// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Settings;
using Vitaru.Settings.Options;

namespace Vitaru.Settings.Overlays
{
    public class MitochondriaSettingsOverlay : InputLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(MitochondriaSettingsOverlay);

        public MitochondriaSettingsOverlay(Game game)
        {
            Size = new Vector2(VitaruSettingsOverlay.WIDTH, VitaruSettingsOverlay.HEIGHT);
            Position = Vector2.Zero;

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    Size = Size,
                },

                new ListLayer<IDrawable2D>
                {
                    Size = Size,
                    Spacing = 4,

                    Children = new IDrawable2D[]
                    {
                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Mitochondria",
                            FontScale = 0.36f,
                        },
                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Graphics",
                            FontScale = 0.24f,
                        },
                        new ToggleOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.Fullscreen)
                        {
                            Text = "Toggle Fullscreen",
                            OnValueChange = value =>
                                Renderer.Window.WindowState = value ? WindowState.Fullscreen : WindowState.Windowed,
                        },
                        new ToggleOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.VSync)
                        {
                            Text = "Toggle VSync",
                        },
                        new SliderOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.DrawFrequency, 30,
                            1000)
                        {
                            Text = "Draw Frequency",
                            OnValueChange = value => Renderer.DrawFrequency = (int)value,
                        },
                        new SliderOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.IdleUpdate, 10, 60)
                        {
                            Text = "Idle Update Frequency",
                            OnValueChange = value => game.IdleUpdate = (int)value,
                        },
                        new SliderOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.IdleDraw, 10, 60)
                        {
                            Text = "Idle Draw Frequency",
                            OnValueChange = value => Renderer.IdleDraw = (int)value,
                        },
                        new ToggleOption<GraphicsSetting>(Game.GraphicsSettings, GraphicsSetting.LimitDrawToUpdate)
                        {
                            Text = "Limit Draw to Update",
                            OnValueChange = value => Renderer.LimitDrawToUpdate = value,
                        },
                        new Text2D
                        {
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Audio",
                            FontScale = 0.24f,
                        },
                        new SliderOption<AudioSetting>(Game.AudioSettings, AudioSetting.Master, 0, 100)
                        {
                            Text = "Master",
                            //OnValueChange = value => TrackManager.CurrentTrack.Gain = value / 100
                        },
                        new SliderOption<AudioSetting>(Game.AudioSettings, AudioSetting.Sounds, 0, 100)
                        {
                            Text = "Sounds",
                            //OnValueChange = value => TrackManager.CurrentTrack.Gain = value / 100
                        },
                        new SliderOption<AudioSetting>(Game.AudioSettings, AudioSetting.Music, 0, 100)
                        {
                            Text = "Music",
                            OnValueChange = value => TrackManager.CurrentTrack.Gain = value / 100,
                        },
                    },
                },
            };
        }
    }
}