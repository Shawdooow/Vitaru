﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;

namespace Vitaru.Graphics
{
    public class ShadeLayer<D> : Layer2D<D>
        where D : IDrawable2D
    {
        public virtual Shades Shade { get; set; }

        public virtual float Intensity { get; set; } = 1;

        public override void Render()
        {
            Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
            Renderer.ShaderManager.UpdateInt("shade", (int) Shade);
            Renderer.ShaderManager.UpdateFloat("intensity", Intensity);
            base.Render();
            Renderer.ShaderManager.UpdateInt("shade", 0);
            Renderer.ShaderManager.UpdateFloat("intensity", 1);
        }
    }

    public enum Shades
    {
        Color = 0,
        None = 0,
        Gray,
        Red,
        Green,
        Blue
    }
}