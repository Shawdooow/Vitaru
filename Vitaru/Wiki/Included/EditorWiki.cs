using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Vitaru.Wiki.Content;

namespace Vitaru.Wiki.Included
{
    public class EditorWiki : WikiPanel
    {
        public override string Name => "Editor";

        public override WikiSection[] GetSections() => new WikiSection[]
        {
            new Features(),
        };

        private class Features : WikiSection
        {
            public override string Name => nameof(Features);

            public override InputLayer<IDrawable2D> GetSection() => new WikiListLayer
            {
                Children = new IDrawable2D[]
                {
                    new Description("Sure, it has some.")
                }
            };
        }
    }
}
