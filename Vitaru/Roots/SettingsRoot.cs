using Vitaru.Roots.Menu;

namespace Vitaru.Roots
{
    public class SettingsRoot : MenuRoot
    {
        public override string Name => nameof(SettingsRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        public SettingsRoot()
        {
            Add(new Version());
        }
    }
}
