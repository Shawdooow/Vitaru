// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;

namespace Vitaru.Play.Characters.Players
{
    public class Seal : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(Seal);

        public readonly Sprite Sign;

        public readonly Text2D EnergyValue;
        public readonly Text2D HealthValue;

        public readonly Text2D RightValue;
        public readonly Text2D LeftValue;

        public readonly CircularMask Circular;

        public Seal()
        {
            Size = new Vector2(256);

            Children = new IDrawable2D[]
            {
                Circular = new CircularMask(),
                Sign = new Sprite
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0.5f,
                },

                EnergyValue = new Text2D(false)
                {
                    Position = new Vector2(-60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopRight,
                    FontScale = 0.25f,
                    Alpha = 0,
                },
                HealthValue = new Text2D(false)
                {
                    Position = new Vector2(60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.25f,
                    Alpha = 0,
                },

                LeftValue = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    FontScale = 0.25f,
                    Alpha = 0.8f,
                },
                RightValue = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    FontScale = 0.25f,
                    Alpha = 0.8f,
                },
            };
        }

        public class CircularMask : CircularLayer<MaskSprite>
        {
            public override string Name { get; set; } = nameof(CircularMask);

            public readonly MaskSprite Energy;
            public readonly MaskSprite Health;

            public float EnergyProgress;
            public float HealthProgress;

            private const float start = 0;
            private const float end = MathF.PI * 2;

            public CircularMask()
            {
                Scale = new Vector2(0.3f);

                Children = new[]
                {
                    Energy = new MaskSprite(),
                    Health = new MaskSprite(),
                };
            }

            public override void Render()
            {
                Renderer.CircularProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.CircularProgram;

                Renderer.ShaderManager.UpdateFloat("startAngle", start);
                Renderer.ShaderManager.UpdateFloat("endAngle", EnergyProgress);
                Energy.Render();

                Renderer.ShaderManager.UpdateFloat("startAngle", HealthProgress);
                Renderer.ShaderManager.UpdateFloat("endAngle", end);
                Health.Render();

                Renderer.SpriteProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
            }
        }

        public class MaskSprite : Sprite
        {
            public MaskSprite() { }

            public MaskSprite(Texture t) : base(t) { }

            public override void Render() => Renderer.Context.Render(this, Renderer.CircularProgram);
        }
    }
}