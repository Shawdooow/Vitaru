using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Text;
using System.Numerics;
using Vitaru.Graphics.UI;
using Vitaru.Roots;
using Vitaru.Roots.Menu;
using static Vitaru.Roots.MainMenu;

namespace Vitaru.Debug
{
    public class DebugMenu : MenuRoot
    {
        protected override bool Parallax => true;

        private readonly Vitaru vitaru;

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

                Text = "Load Game",

                OnClick = () => AddRoot(new MainMenu(vitaru)),
            });

            Add(Back = new Exit(vitaru));

            Add(new Version());

            Add(new Text2D
            {
                Y = -300,
                Text = Vitaru.ALKI > 0 ? Vitaru.ALKI == 2 ? "Rhize" : "Alki" : "Vitaru",
            });

            Renderer.Window.CursorHidden = true;
        }

        protected override void DropRoot()
        {
            //base.DropRoot();
        }
    }
}
