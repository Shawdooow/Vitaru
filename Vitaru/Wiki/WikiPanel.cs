using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Wiki
{
    public abstract class WikiPanel : IHasName, IHasDescription, IHasIcon
    {
        public abstract string Name { get; }

        public virtual string Description => $"The {Name} Wiki.";

        public virtual Texture Icon { get; }

        public abstract WikiSection[] GetSections();
    }
}
