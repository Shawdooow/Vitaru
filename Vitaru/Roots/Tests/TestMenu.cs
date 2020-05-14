// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Application.Timing;
using Prion.Game;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Overlays;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Roots.Multi;
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
            Button edit;
            Button multi;
            Button wiki = null;

            Add(play = new Button
            {
                Position = new Vector2(0, -200),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Play",

                OnClick = () => AddRoot(new PlayTest(seek, track))
            });
            Add(multi = new Button
            {
                Position = new Vector2(0, -75),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Multi",

                OnClick = () => AddRoot(new MultiMenu())
            });
            Add(edit = new Button
            {
                Position = new Vector2(0, 75),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "Edit",

                OnClick = () => AddRoot(new EditorTest())
            });
            Add(wiki = new Button
            {
                Position = new Vector2(0, 200),
                Size = new Vector2(100, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                Text = "?",

                OnClick = () => wiki.Text = "Wiki"
            });

            Add(new SpriteText
            {
                Position = new Vector2(10),
                ParentOrigin = Mounts.TopLeft,
                Origin = Mounts.TopLeft,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru"
            });

            play.BackgroundSprite.Color = Color.ForestGreen;
            edit.BackgroundSprite.Color = Color.DarkTurquoise;
            multi.BackgroundSprite.Color = Color.DarkMagenta;
            wiki.BackgroundSprite.Color = Color.DarkGoldenrod;

            Add(new FPSOverlay());
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();
            track = new Track(Vitaru.ALKI ? Track.GetEndgame() : Track.GetBells(), seek);
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