using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vitaru.Gamemodes;
using Vitaru.Play.Characters.Players;
using Vitaru.Roots;

namespace Vitaru.Debug
{
    internal class PlayerRenderTest : MenuRoot
    {
        Layer2D<IDrawable2D> layer;
        DrawablePlayer drawable;

        public PlayerRenderTest()
        {
        }

        public override void RenderingLoadingComplete()
        {
            base.RenderingLoadingComplete();

            Add(layer = new Layer2D<IDrawable2D>
            {
                ParentOrigin = Mounts.Center,
                Origin = Mounts.Center
            });

            //Load the Drawable here
            drawable = new DrawablePlayer(GamemodeStore.SelectedGamemode.Players[0].Value, layer);
            drawable.Position = Vector2.Zero;
            drawable.HitboxAlpha = 1;
        }

        public override void Render2D()
        {
            base.Render2D();
        }
    }
}
