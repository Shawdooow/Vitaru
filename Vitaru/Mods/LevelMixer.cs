using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Roots;
using Vitaru.Tracks;

namespace Vitaru.Mods
{
    public class LevelMixer : Mod
    {
        public override Button GetMenuButton() =>
            new Button
            {
                Position = new Vector2(0, -200),
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.DarkMagenta
                },

                Text = "Mixer",
            };

        public override Root GetRoot() => new Mixer();

        private class Mixer : MenuRoot
        {
            protected override bool UseLevelBackground => true;

            public Mixer()
            {
                Add(new Button
                {
                    Position = new Vector2(240, 0),
                    Size = new Vector2(100, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Dim =
                    {
                        Alpha = 0.5f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.Yellow
                    },
                    SpriteText =
                    {
                        TextScale = 0.5f
                    },

                    Text = "1.5x",
                    OnClick = () => setRate(1.5f)
                });

                Add(new Button
                {
                    Position = new Vector2(120, 0),
                    Size = new Vector2(80, 80),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Dim =
                    {
                        Alpha = 0.5f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.OrangeRed
                    },
                    SpriteText =
                    {
                        TextScale = 0.25f
                    },

                    Text = "+ 0.05x",
                    OnClick = () => setRate(TrackManager.CurrentTrack.Pitch + 0.05f)
                });

                Add(new Button
                {
                    Size = new Vector2(100, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Dim =
                    {
                        Alpha = 0.5f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.Yellow
                    },
                    SpriteText =
                    {
                        TextScale = 0.5f
                    },

                    Text = "1x",
                    OnClick = () => setRate(1f)
                });

                Add(new Button
                {
                    Position = new Vector2(-120, 0),
                    Size = new Vector2(80, 80),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Dim =
                    {
                        Alpha = 0.5f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.OrangeRed
                    },
                    SpriteText =
                    {
                        TextScale = 0.25f
                    },

                    Text = "- 0.05x",
                    OnClick = () => setRate(TrackManager.CurrentTrack.Pitch - 0.05f)
                });

                Add(new Button
                {
                    Position = new Vector2(-240, 0),
                    Size = new Vector2(100, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Dim =
                    {
                        Alpha = 0.5f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.Yellow
                    },
                    SpriteText =
                    {
                        TextScale = 0.5f
                    },

                    Text = "0.75x",
                    OnClick = () => setRate(0.75f)
                });
            }

            private void setRate(float rate)
            {
                TrackManager.CurrentTrack.Pitch = rate;
            }
        }
    }
}
