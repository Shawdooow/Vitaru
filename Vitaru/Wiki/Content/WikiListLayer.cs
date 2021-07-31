using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Vitaru.Roots;

namespace Vitaru.Wiki.Content
{
    public class WikiListLayer : ListLayer<IDrawable2D>
    {
        public WikiListLayer()
        {
            ParentOrigin = Mounts.Center;
            Origin = Mounts.Center;

            Size = new Vector2(WikiRoot.WIDTH - 40, WikiRoot.HEIGHT - 80);
            Spacing = 6;
        }
    }
}
