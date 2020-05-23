using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.UserInterface;

namespace Vitaru.Mods
{
    public abstract class Mod
    {
        public virtual Button GetMenuButton() => null;

        public virtual Root GetRoot() => null;

        //public virtual ModSubSection GetSettings() => null;

        //public virtual WikiSet GetWikiSet() => null;

        public virtual void LoadingComplete() { }
    }
}
