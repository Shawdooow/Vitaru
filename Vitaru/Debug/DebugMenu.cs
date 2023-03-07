using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using System.Numerics;
using Vitaru.Graphics.UI;
using Vitaru.Levels;
using Vitaru.Roots;
using Vitaru.Roots.Menu;
using Vitaru.Tracks;
using static Vitaru.Roots.MainMenu;

namespace Vitaru.Debug
{
    public class DebugMenu : MenuRoot
    {
        protected override bool Parallax => true;

        private readonly Vitaru vitaru;

        private VitaruTrackController controller;

        private const int x = 100;
        private const int y = 50;
        private const int width = 180;
        private const int height = 80;

        public DebugMenu(Vitaru vitaru)
        {
            this.vitaru = vitaru;
        }

        public override void RenderingPreLoading()
        {
            base.RenderingPreLoading();

            Add(new VitaruButton
            {
                Position = new Vector2(-x, -y - height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                Color = ThemeManager.PrimaryColor,

                Text = "Vitaru",

                OnClick = () => AddRoot(new MainMenu(vitaru)),
            });
            Add(new VitaruButton
            {
                Position = new Vector2(x, -y - height / 2),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                Color = ThemeManager.SecondaryColor,

                Text = "PlayerRenderTest",

                OnClick = () => AddRoot(new PlayerRenderTest()),
            });
            Add(new VitaruButton
            {
                Position = new Vector2(-x, 0),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                Color = ThemeManager.TrinaryColor,

                Text = "",

                Disabled = true,
                //OnClick = () =>
                //{
                //    if (TrackManager.CurrentTrack != null)
                //        AddRoot(new EditorRoot());
                //},
            });
            Add(new VitaruButton
            {
                Position = new Vector2(x, 0),
                Size = new Vector2(width, height),

                Background = Game.TextureStore.GetTexture("square.png"),
                Color = ThemeManager.QuadnaryColor,

                Text = "",

                Disabled = true,
                //OnClick = () =>
                //{
                //    if (TrackManager.CurrentTrack != null)
                //        AddRoot(new ModsTest());
                //},
            });

            Add(new TrackSelect());

            Add(Back = new Exit(vitaru));

            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight,
            });

            Add(new Version());

            Add(new Text2D
            {
                Y = -300,
                Text = "Debug Menu",
            });

            Renderer.Window.CursorHidden = true;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            controller.OnPrimeTrackManager = () =>
            {
                LevelPack p = LevelStore.GetRandomLevelPack(null);

                if (Vitaru.ALKI == 1)
                {
                    for (int i = 0; i < LevelStore.LoadedLevels.Count; i++)
                        if (LevelStore.LoadedLevels[i].Title == "Alki Bells")
                            p = LevelStore.LoadedLevels[i];
                }
                else if (Vitaru.ALKI == 2)
                {
                    for (int i = 0; i < LevelStore.LoadedLevels.Count; i++)
                        if (LevelStore.LoadedLevels[i].Title == "Alki (All Rhize Remix)")
                            p = LevelStore.LoadedLevels[i];
                }

                LevelStore.SetLevelPack(p);

                return LevelStore.CurrentLevel.Metadata;
            };

            controller.PrimeTrackManager();
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.SetPositionalDefaults();
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();
        }

        protected override void DropRoot()
        {
            //base.DropRoot();
        }
    }
}
