using Vitaru.Roots.Menu;
using Vitaru.Settings.Overlays;

namespace Vitaru.Roots
{
    public class SettingsRoot : MenuRoot
    {
        public override string Name => nameof(SettingsRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        public SettingsRoot(Vitaru vitaru)
        {
            Add(new NucleusSettingsOverlay(vitaru));
            Add(new Version());
        }
    }
}
