using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Mods;

namespace Vitaru.Roots.Tests
{
    public class ModsTest : MenuRoot
    {
        protected override bool UseLevelBackground => true;

        public ModsTest()
        {
            foreach (Mod mod in Modloader.LoadedMods)
            {
                Button b = mod.GetMenuButton();
                Root r = mod.GetRoot();
                if (b != null && r != null)
                {
                    r.Dispose();
                    Add(b);
                    b.OnClick += () => AddRoot(mod.GetRoot());
                }
            }
        }
    }
}
