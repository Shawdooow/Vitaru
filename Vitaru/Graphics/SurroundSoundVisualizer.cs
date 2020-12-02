using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Transforms;
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
                },
                right = new Bumper
                {
                    Position = TrackManager.CurrentTrack.Source.RightPosition.XZ(),
                },
            };
        }

        public void OnNewBeat()
        {
            left.Pulse();
            right.Pulse();
        }

        public class Bumper : Layer2D<IDrawable2D>
        {
            private Sprite glow1;
            private Sprite glow2;
            private Sprite circle;

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
                    circle = new Sprite
                    {
                        Texture = Game.TextureStore.GetTexture("Cursor\\ring.png"),
                        Size = new Vector2(128),
                        Scale = new Vector2(0.5f),
                    },
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

                circle.ScaleTo(new Vector2(0.45f), 20, Easings.OutQuart).OnComplete(() => 
                    circle.ScaleTo(new Vector2(0.5f), length - 20, Easings.OutSine));
            }
        }
    }
}
