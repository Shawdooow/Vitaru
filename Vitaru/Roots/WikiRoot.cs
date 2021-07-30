using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Vitaru.Roots.Menu;
using Vitaru.Roots.Wiki;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class WikiRoot : MenuRoot
    {
        public override string Name => nameof(WikiRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private VitaruTrackController controller;

        public WikiRoot()
        {
            Add(new Index());
            Add(new Version());
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
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }
    }
}
