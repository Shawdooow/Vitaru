using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        public MainMenuRoot()
        {
            Layers.ShaderProgram = Sprite.SpriteProgram;
            Add(new Layer2D<Sprite>
            {
                ShaderProgram = Sprite.SpriteProgram,
                ParentScaling = Axes.Both,

                Child = new Sprite(Game.TextureStore.GetTexture("medicine.png"))
                {
                    ParentScaling = Axes.Both,
                    Size = new Vector2(0.5f),
                    Color = Color.MidnightBlue,
                }
            });
        }
    }
}
