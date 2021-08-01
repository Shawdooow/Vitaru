using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Vitaru.Wiki.Content;

namespace Vitaru.Wiki.Included
{
    public class MultiplayerWiki : WikiPanel
    {
        public override string Name => "Multiplayer";

        public override WikiSection[] GetSections() => new WikiSection[]
        {
            new Online(),
        };

        private class Online : WikiSection
        {
            public override string Name => nameof(Online);

            public override InputLayer<IDrawable2D> GetSection() => new WikiListLayer
            {
                Children = new IDrawable2D[]
                {
                    new Description("Online multiplayer doesn't work at all yet."),
                }
            };
        }
    }
}
