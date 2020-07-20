using System.Drawing;
using System.Numerics;
using Prion.Golgi.Utilities;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Roots;

namespace Vitaru.Mods.Included
{
    public sealed class Prion : Mod
    {
        public override bool Disabled => !Vitaru.EXPERIMENTAL;

        public override Button GetMenuButton() =>
            new Button
            {
                ParentOrigin = Mounts.BottomRight,
                Origin = Mounts.BottomRight,
                Position = new Vector2(-10),
                Size = new Vector2(80, 40),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.Black
                },

                Text = "Prion",
                SpriteText =
                {
                    TextScale = 0.35f
                }
            };

        public override Root GetRoot() => new PrionRoot();

        private class PrionRoot : MenuRoot
        {
            public override string Name => nameof(PrionRoot);

            protected override bool UseLevelBackground => true;

            protected override bool Parallax => true;

            public PrionRoot()
            {
                Add(new Button
                {
                    Position = new Vector2(0, -180),
                    Size = new Vector2(200, 100),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    BackgroundSprite =
                    {
                        Color = Color.Gold
                    },

                    Text = "Run All",

                    OnClick = () => Benchmarks.All()
                });
            }
        }
    }
}
