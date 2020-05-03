using System.Drawing;
using System.Numerics;
using Prion.Application.Debug;
using Prion.Game;
using Prion.Game.Audio;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Editor;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        private readonly SeekableClock seek;
        private readonly AudioDevice device;
        private RepeatableSample track;
        
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

            Add(new Button
            {
                //Position = new Vector2(0, 200),

                Background = Game.TextureStore.GetTexture("Shrek.png"),

                OnClick = () => AddRoot(new PlayTest(seek, track))
            });
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            seek.Start();
            track = new RepeatableSample(Vitaru.ALKI ? "alki endgame.wav" : "alki bells.mp3", seek);
        }

        public override void Update()
        {
            seek.NewFrame();
            track?.CheckRepeat();
            base.Update();
        }
    }
}
