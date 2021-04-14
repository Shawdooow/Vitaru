using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;

namespace Vitaru.Roots.Menu
{
    public class CharacterStats : ListLayer<IDrawable2D>
    {
        public CharacterStats()
        {
            Position = new Vector2();
            Size = new Vector2();

            Children = new[]
            {
                new Text2D
                {

                }
            };
        }
    }
}
