using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;

namespace Vitaru.Wiki.Included
{
    public class VitaruWiki : WikiPanel
    {
        public override string Name => "Vitaru";

        public override WikiSection[] GetSections() => new[]
        {
            new About()
        };

        private class About : WikiSection
        {
            public override string Name => "About";

            public override InputLayer<IDrawable2D> GetSection() => new InputLayer<IDrawable2D>()
            {

            };
        }
    }
}
