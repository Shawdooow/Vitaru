using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Vitaru.Settings.Overlays;
using Vitaru.Tracks;
using Version = Vitaru.Roots.Menu.Version;

namespace Vitaru.Roots
{
    public class SettingsRoot : MenuRoot
    {
        public override string Name => nameof(SettingsRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private VitaruTrackController controller;

        private Vitaru vitaru;

        public SettingsRoot(Vitaru vitaru)
        {
            this.vitaru = vitaru;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight,

                PassDownInput = false,
                Alpha = 0
            });

            Add(new NucleusSettingsOverlay(vitaru));
            Add(new MitochondriaSettingsOverlay(vitaru));
            Add(new VitaruSettingsOverlay());
            Add(new Version());
            Remove(Cursor, false);
            Add(Cursor);
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            vitaru = null;
        }
    }
}
