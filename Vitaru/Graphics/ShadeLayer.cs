﻿using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;

namespace Vitaru.Graphics
{
    public class ShadeLayer<D> : Layer2D<D>
        where D : IDrawable2D
    {
        public virtual Shades Shade { get; set; }

        public override void Render()
        {
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
            Renderer.ShaderManager.UpdateInt("shade", (int)Shade);
            base.Render();
            Renderer.ShaderManager.UpdateInt("shade", 0);
        }
    }

    public enum Shades
    {
        Color = 0,
        None = 0,
        Gray,
        Red,
        Green,
        Blue,
    }
}
