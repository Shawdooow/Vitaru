using Vitaru.Roots.Menu;
using Vitaru.Roots.Wiki;

namespace Vitaru.Roots
{
    public class WikiRoot : MenuRoot
    {
        public override string Name => nameof(WikiRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        public WikiRoot()
        {
            Add(new Index());
            Add(new Version());
        }
    }
}
