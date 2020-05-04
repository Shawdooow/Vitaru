// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Audio;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Editor;
using Vitaru.Server.Track;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        private readonly Box shade;

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
                    shade = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    }
                }
            });

            Button play;
            Button edit;

            Add(play = new Button
            {
                Position = new Vector2(200, 0),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),

                OnClick = () => AddRoot(new PlayTest(seek, track))
            });
            Add(edit = new Button
            {
                Position = new Vector2(-200, 0),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),

                OnClick = () => AddRoot(new EditorTest())
            });

            Add(new SpriteText
            {
                Position = new Vector2(200, 0),
                Text = "Play",
                TextScale = 0.5f,
            });
            Add(new SpriteText
            {
                Position = new Vector2(-200, 0),
                Text = "Edit",
                TextScale = 0.5f,
            });

            Add(new SpriteText
            {
                Position = new Vector2(-20, 10),
                ParentOrigin = Mounts.TopLeft,
                Origin = Mounts.TopLeft,
                Text = Vitaru.ALKI ? "Alki" : "Vitaru",
            });

            play.BackgroundSprite.Color = Color.GreenYellow;
            edit.BackgroundSprite.Color = Color.DarkTurquoise;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();
            track = new Track(Vitaru.ALKI ? Track.GetEndgame() : Track.GetBells(), seek);
        }

        protected override void OnResume()
        {
            base.OnResume();
            shade.Alpha = 0.8f;
            shade.FadeTo(0.5f, 1000);
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