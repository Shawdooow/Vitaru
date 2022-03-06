// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus.Utilities;

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
                Circular = new CircularMask(player),
                Sign = new Sprite(Game.TextureStore.GetTexture(player.Seal))
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0.5f,
                    Color = player.SecondaryColor,
                },

                EnergyValue = new Text2D(false)
                {
                    Position = new Vector2(-60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopRight,
                    FontScale = 0.25f,
                    Alpha = 0,
                    Color = player.ComplementaryColor,
                },
                HealthValue = new Text2D(false)
                {
                    Position = new Vector2(60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.25f,
                    Alpha = 0,
                    Color = player.ComplementaryColor,
                },

                LeftValue = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    FontScale = 0.25f,
                    Alpha = 0.8f,
                    Color = player.ComplementaryColor,
                },
                RightValue = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    FontScale = 0.25f,
                    Alpha = 0.8f,
                    Color = player.ComplementaryColor,
                },
            };
        }

        public class CircularMask : CircularLayer<MaskSprite>
        {
            public override string Name { get; set; } = nameof(CircularMask);

            private readonly MaskSprite energy;
            private readonly MaskSprite health;

            public CircularMask()
            {
                Scale = new Vector2(0.3f);

                Children = new[]
                {
                    energy = new MaskSprite(Game.TextureStore.GetTexture(player.EnergyRing))
                    {
                        Color = player.SecondaryColor,
                    },
                    health = new MaskSprite(Game.TextureStore.GetTexture(player.HealthRing))
                    {
                        Color = player.PrimaryColor,
                    },
                };
            }

            private const float start = 0;
            private const float end = MathF.PI * 2;

            public override void Render()
            {
                Renderer.CircularProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.CircularProgram;

                Renderer.ShaderManager.UpdateFloat("startAngle", start);
                Renderer.ShaderManager.UpdateFloat("endAngle",
                    PrionMath.Remap(player.Energy, 0, player.EnergyCapacity, start, end));
                energy.Render();

                Renderer.ShaderManager.UpdateFloat("startAngle",
                    PrionMath.Remap(player.Health, 0, player.HealthCapacity, end, start));
                Renderer.ShaderManager.UpdateFloat("endAngle", end);
                health.Render();

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