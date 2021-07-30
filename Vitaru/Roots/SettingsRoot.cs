using Vitaru.Roots.Menu;
using Vitaru.Settings.Overlays;

namespace Vitaru.Roots
{
    public class SettingsRoot : MenuRoot
    {
        public override string Name => nameof(SettingsRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private Vitaru vitaru;

        public SettingsRoot(Vitaru vitaru)
        {
            this.vitaru = vitaru;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Add(new NucleusSettingsOverlay(vitaru));
            Add(new MitochondriaSettingsOverlay(vitaru));
            Add(new VitaruSettingsOverlay());
            Add(new Version());
            Remove(Cursor, false);
            Add(Cursor);
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            vitaru = null;
        }
    }
}
