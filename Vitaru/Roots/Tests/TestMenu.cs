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
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        private readonly SeekableClock seek;
        private readonly AudioDevice device;
        private Track track;

        public TestMenu()
        {
            seek = new SeekableClock();
            device = new AudioDevice();

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

            Button play;
            Button multi;
            Button edit;
            Button wiki = null;
            Button setting;

            Add(play = new Button
            {
                Position = new Vector2(0, -170),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Play",

                OnClick = () => AddRoot(new PlayTest(seek, track))
            });
            Add(multi = new Button
            {
                Position = new Vector2(0, -60),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Multi",

                OnClick = () => AddRoot(new MultiMenu())
            });
            Add(edit = new Button
            {
                Position = new Vector2(0, 60),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Edit",

                OnClick = () => AddRoot(new EditorTest())
            });
            Add(wiki = new Button
            {
                Position = new Vector2(0, 170),
                Size = new Vector2(100, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "?",

                OnClick = () => wiki.Text = "Wiki"
            });

            Add(setting = new Button
            {
                ParentOrigin = Mounts.CenterRight,
                Origin = Mounts.CenterRight,

                Size = new Vector2(100, 200),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Settings",
                SpriteText =
                {
                    TextScale = 0.25f,
                },

                OnClick = () =>
                {
                    //settings.Toggle();
                }
            });

            Add(new SpriteText
            {
                Position = new Vector2(10),
                ParentOrigin = Mounts.TopLeft,
                Origin = Mounts.TopLeft,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru"
            });

            play.BackgroundSprite.Color = Color.ForestGreen;
            multi.BackgroundSprite.Color = Color.DarkMagenta;
            edit.BackgroundSprite.Color = Color.DarkTurquoise;
            wiki.BackgroundSprite.Color = Color.DarkGoldenrod;

            setting.BackgroundSprite.Color = Color.DarkSlateBlue;

            Add(new FPSOverlay());
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();
            LevelTrack t = Track.GetBells();
            Storage storage = Game.SoundStorage;

            if (LevelStore.LoadedLevels.Count > 0 && !Vitaru.ALKI)
            {
                t = LevelStore.LoadedLevels[PrionMath.RandomNumber(0, LevelStore.LoadedLevels.Count)].Levels[0].LevelTrack;
                storage = Vitaru.LevelStorage.GetStorage($"{t.Name}");
                Logger.Log($"Playing {t.Name}");
            }

            track = new Track(Vitaru.ALKI ? Track.GetEndgame() : t, seek, storage);
        }

        public override void Update()
        {
            seek.NewFrame();
            track?.CheckRepeat();
            track?.CheckNewBeat();
            base.Update();
        }
    }
}