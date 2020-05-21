// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Core.Debug;
using Prion.Core.IO;
using Prion.Core.Timing;
using Prion.Core.Utilities;
using Prion.Game;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Overlays;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Levels;
using Vitaru.Roots.Multi;
using Vitaru.Server.Track;
using Vitaru.Settings;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        private readonly Button next;
        private readonly SpriteText song;

        private readonly SeekableClock seek;

        public TestMenu()
        {
            seek = new SeekableClock();
            

            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    }
                }
            });

            Add(new Button
            {
                Position = new Vector2(0, -180),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.ForestGreen
                },

                Text = "Play",

                OnClick = () => AddRoot(new PlayTest(seek))
            });
            Add(new Button
            {
                Position = new Vector2(0, -60),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta
                },

                Text = "Multi",

                OnClick = () => AddRoot(new MultiMenu())
            });
            Add(new Button
            {
                Position = new Vector2(0, 60),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkTurquoise
                },

                Text = "Edit",

                OnClick = () => AddRoot(new EditorTest())
            });
            Add(new Button
            {
                Position = new Vector2(0, 180),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkGoldenrod
                },

                Text = "Mods",

                OnClick = () => AddRoot(new ModsTest())
            });

            Add(next = new Button
            {
                Position = new Vector2(-10, 40),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                Size = new Vector2(160, 90),

                Dim =
                {
                    Alpha = 0.8f
                },
                SpriteText =
                {
                    TextScale = 0.25f
                },

                Text = "Next",

                OnClick = TrackManager.NextTrack
            });

            //Add(new WikiOverlay());
            Add(new SettingsOverlay());

            Add(song = new SpriteText
            {
                Position = new Vector2(-10, 10),
                ParentOrigin = Mounts.TopRight,
                Origin = Mounts.TopRight,
                TextScale = 0.25f,
            });

            Add(new SpriteText
            {
                Position = new Vector2(10),
                ParentOrigin = Mounts.TopLeft,
                Origin = Mounts.TopLeft,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru"
            });

            Add(new FPSOverlay());
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();

            LevelTrack t = LevelStore.LoadedLevels[PrionMath.RandomNumber(0, LevelStore.LoadedLevels.Count)].Levels[0]
                .LevelTrack;

            TrackManager.OnTrackChange += track =>
            {
                song.Text = $"Now Playing: {track.Level.Name}";
                next.Background = new Texture(Vitaru.LevelStorage.GetStream($"{track.Level.Name}\\{track.Level.Image}"), track.Level.Name);
            };
            TrackManager.SetTrack(t, seek);
        }

        public override void Update()
        {
            seek.NewFrame();
            if (TrackManager.CurrentTrack != null)
                TrackManager.TryNextTrack();
            base.Update();
        }
    }
}