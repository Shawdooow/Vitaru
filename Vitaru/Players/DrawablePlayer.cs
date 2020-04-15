using System.Drawing;
using System.Numerics;
using Prion.Game;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Sprites;

namespace Vitaru.Players
{
    public class DrawablePlayer : Sprite
    {
        public DrawablePlayer() : base(Game.TextureStore.GetTexture("Sakuya Izayoi.png"))
        {
            ParentOrigin = Mounts.Center;
            Color = Color.Blue;
        }

        public override void PreRender()
        {
            base.PreRender();
            UpdateTranslateTransform();
        }
    }
}
