using System.Drawing;
using System.Numerics;
using Prion.Application.Debug;
using Prion.Game;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.UserInterface;

namespace Vitaru.Roots.Tests
{
    public class TestMenu : Root
    {
        public TestMenu()
        {
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

                OnClick = () => SetRoot(new PlayTest())
            });
        }
    }
}
