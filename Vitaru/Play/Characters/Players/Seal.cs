// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Input;

namespace Vitaru.Play.Characters.Players
{
    public class Seal : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(Seal);

        public readonly Sprite Sign;
        public readonly Sprite Reticle;

        public readonly Text2D EnergyValue;
        public readonly Text2D HealthValue;

        public readonly Text2D RightValue;
        public readonly Text2D LeftValue;

        private readonly CircularMask circular;

        private Player player;

        private const double duration = 200;

        public Seal(Player player)
        {
            this.player = player;
            Texture reticle = Game.TextureStore.GetTexture(player.Reticle);
            Size = reticle.Size / 4;

            Children = new IDrawable2D[]
            {
                circular = new CircularMask(player),
                Reticle = new Sprite(reticle)
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0f,
                    Color = player.SecondaryColor,
                },
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

        public void Update()
        {
            float amount = player.GetBind(VitaruActions.Sneak) ? 1500 : 1000;

            if (!player.SpellActive)
                Sign.Rotation += (float)(player.Clock.LastElapsedTime / amount * player.SealRotationSpeed);
            else
                Sign.Rotation -= (float)(player.Clock.LastElapsedTime / amount * player.SealRotationSpeed);

            Reticle.Rotation =
                (float)Math.Atan2(InputManager.Mouse.Position.Y - player.Position.Y,
                    InputManager.Mouse.Position.X - player.Position.X) +
                (float)Math.PI / 2f;

            EnergyValue.Text = $"{Math.Round(player.Energy, 0)}SP";
            HealthValue.Text = $"{Math.Round(player.Health, 0)}HP";

            Sign.Alpha = PrionMath.Remap(player.Energy, 0, player.EnergyCapacity, 0.1f);
        }

        public void Shoot(double flash)
        {
            if (player.GetBind(VitaruActions.Sneak))
            {
                Reticle.Alpha = 1f;
                Reticle.FadeTo(Sign.Alpha, flash, Easings.OutCubic);
            }
        }

        public void Pressed(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.FadeTo(Sign.Alpha, duration);
                Reticle.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                Sign.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                circular.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                EnergyValue.FadeTo(0.8f, duration);
                EnergyValue.MoveTo(new Vector2(-40, 40), duration, Easings.OutCubic);

                HealthValue.FadeTo(0.8f, duration);
                HealthValue.MoveTo(new Vector2(40, 40), duration, Easings.OutCubic);

                LeftValue.MoveTo(new Vector2(40, 0), duration, Easings.OutCubic);
                RightValue.MoveTo(new Vector2(-40, 0), duration, Easings.OutCubic);
            }
        }

        public void Released(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.FadeTo(0f, duration);
                Reticle.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                Sign.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                circular.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                EnergyValue.FadeTo(0, duration);
                EnergyValue.MoveTo(new Vector2(-60, 10), duration, Easings.OutCubic);

                HealthValue.FadeTo(0, duration);
                HealthValue.MoveTo(new Vector2(60, 10), duration, Easings.OutCubic);

                LeftValue.MoveTo(Vector2.Zero, duration, Easings.OutCubic);
                RightValue.MoveTo(Vector2.Zero, duration, Easings.OutCubic);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            player = null;
            base.Dispose(isDisposing);
        }

        private class CircularMask : CircularLayer<MaskSprite>
        {
            public override string Name { get; set; } = nameof(CircularMask);

            private readonly Player player;

            private readonly MaskSprite outer;
            private readonly MaskSprite inner;

            public CircularMask(Player player)
            {
                this.player = player;

                Scale = new Vector2(0.3f);

                Children = new[]
                {
                    outer = new MaskSprite(Game.TextureStore.GetTexture(player.EnergyRing))
                    {
                        Color = player.SecondaryColor,
                    },
                    inner = new MaskSprite(Game.TextureStore.GetTexture(player.HealthRing))
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
                outer.Render();

                Renderer.ShaderManager.UpdateFloat("startAngle",
                    PrionMath.Remap(player.Health, 0, player.HealthCapacity, end, start));
                Renderer.ShaderManager.UpdateFloat("endAngle", end);
                inner.Render();

                Renderer.SpriteProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
            }
        }

        private class MaskSprite : Sprite
        {
            public MaskSprite() { }

            public MaskSprite(Texture t) : base(t) { }

            public override void Render() => Renderer.Context.Render(this, Renderer.CircularProgram);
        }
    }
}