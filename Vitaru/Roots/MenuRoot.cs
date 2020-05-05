using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Roots
{
    public abstract class MenuRoot : Root
    {
        protected readonly Box Shade;

        protected MenuRoot()
        {
            Add(new SpriteLayer
            {
                Children = new[]
                {
                    new Sprite(Vitaru.GetBackground())
                    {
                        Scale = new Vector2(0.75f)
                    },
                    Shade = new Box
                    {
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5)
                    }
                }
            });
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Escape:
                    DropRoot();
                    break;
            }
        }
    }
}
