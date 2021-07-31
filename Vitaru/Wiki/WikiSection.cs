using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Wiki
{
    public abstract class WikiSection : IHasName
    {
        public abstract string Name { get; }

        public abstract InputLayer<IDrawable2D> GetSection();
    }
}