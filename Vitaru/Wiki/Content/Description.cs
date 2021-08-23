using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Roots;

namespace Vitaru.Wiki.Content
{
    public class Description : Text2D
    {
        public Description(string description) : base(description.Length)
        {
            ParentOrigin = Mounts.TopLeft;
            Origin = Mounts.TopLeft;

            Text = description;
            FontScale = 0.24f;
            FixedWidth = WikiRoot.WIDTH - 40;
            X = 20;
        }
    }
}
