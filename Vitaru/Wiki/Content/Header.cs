using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Roots;

namespace Vitaru.Wiki.Content
{
    public class Header : Text2D
    {
        public Header(string header) : base(header.Length)
        {
            ParentOrigin = Mounts.TopLeft;
            Origin = Mounts.TopLeft;

            Text = header;
            FontScale = 0.36f;
            FixedWidth = WikiRoot.WIDTH - 40;
            X = 20;
        }
    }
}
