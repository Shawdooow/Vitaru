using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Vitaru.Roots.Menu;
using Vitaru.Tracks;
using Vitaru.Wiki;

namespace Vitaru.Roots
{
    public class WikiRoot : MenuRoot
    {
        public override string Name => nameof(WikiRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private Layer2D<IDrawable2D> panel;

        private VitaruTrackController controller;

        public WikiRoot()
        {
            Index index = new();
            Add(index);
            Add(panel = new Layer2D<IDrawable2D>());
            Add(new Version());

            index.OnSetPanel += p =>
            {

            };
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
            controller.TryNextLevel();
        }
    }
}
