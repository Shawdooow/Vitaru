// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.Transforms;
using Vitaru.Input;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class Seal : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(Seal);

        public readonly Sprite Sign;
        public readonly Sprite Reticle;

        public readonly SpriteText EnergyValue;
        public readonly SpriteText HealthValue;

        public readonly SpriteText RightValue;
        public readonly SpriteText LeftValue;

        private readonly CircularMask circular;

        private Player player;

        private const double duration = 200;

        public Seal(Player player)
        {
            this.player = player;
            Texture reticle = Game.TextureStore.GetTexture("Gameplay\\reticle.png");
            Size = reticle.Size / 4;

            Children = new IDrawable2D[]
            {
                circular = new CircularMask(player),
                Reticle = new Sprite(reticle)
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0f,
                    Color = player.PrimaryColor
                },
                Sign = new Sprite(Game.TextureStore.GetTexture("Gameplay\\seal.png"))
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0.5f,
                    Color = player.PrimaryColor
                },

                EnergyValue = new SpriteText
                {
                    Position = new Vector2(-60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopRight,
                    TextScale = 0.25f,
                    Alpha = 0
                    //Color = player.SecondaryColor,
                },
                HealthValue = new SpriteText
                {
                    Position = new Vector2(60, 10),
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopLeft,
                    TextScale = 0.25f,
                    Alpha = 0
                    //Color = player.SecondaryColor,
                },

                LeftValue = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    TextScale = 0.25f,
                    Alpha = 0.8f
                    //Color = player.SecondaryColor,
                },
                RightValue = new SpriteText
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    TextScale = 0.25f,
                    Alpha = 0.8f
                    //Color = player.SecondaryColor,
                }
            };
        }

        public void Update()
        {
            float speed = player.InputHandler.Actions[VitaruActions.Sneak] ? 1500 : 1000;

            if (!player.SpellActive)
                Sign.Rotation -= (float) (player.Clock.LastElapsedTime / speed);
            else
                Sign.Rotation += (float) (player.Clock.LastElapsedTime / speed);

            Reticle.Rotation =
                (float) Math.Atan2(player.Cursor.Y - player.Position.Y, player.Cursor.X - player.Position.X) +
                (float) Math.PI / 2f;

            EnergyValue.Text = $"{Math.Round(player.Energy, 0)}/{player.EnergyCapacity}J";
            HealthValue.Text = $"{Math.Round(player.Health, 0)}/{player.HealthCapacity}HP";

            Sign.Alpha = PrionMath.Scale(player.Energy, 0, player.EnergyCapacity, 0.1f);
        }

        public void Shoot(double flash)
        {
            if (player.InputHandler.Actions[VitaruActions.Sneak])
            {
                Reticle.Alpha = 1f;
                Reticle.FadeTo(Sign.Alpha, flash, Easings.OutCubic);
            }
        }

        public void Pressed(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.ClearTransforms();
                Reticle.FadeTo(Sign.Alpha, duration);
                Reticle.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                circular.ClearTransforms();
                circular.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                EnergyValue.ClearTransforms();
                EnergyValue.FadeTo(0.8f, duration);
                EnergyValue.MoveTo(new Vector2(-40, 40), duration, Easings.OutCubic);

                HealthValue.ClearTransforms();
                HealthValue.FadeTo(0.8f, duration);
                HealthValue.MoveTo(new Vector2(40, 40), duration, Easings.OutCubic);

                LeftValue.ClearTransforms();
                LeftValue.MoveTo(new Vector2(40, 0), duration, Easings.OutCubic);

                RightValue.ClearTransforms();
                RightValue.MoveTo(new Vector2(-40, 0), duration, Easings.OutCubic);
            }
        }

        public void Released(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.ClearTransforms();
                Reticle.FadeTo(0f, duration);
                Reticle.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                circular.ClearTransforms();
                circular.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                EnergyValue.ClearTransforms();
                EnergyValue.FadeTo(0, duration);
                EnergyValue.MoveTo(new Vector2(-60, 10), duration, Easings.OutCubic);

                HealthValue.ClearTransforms();
                HealthValue.FadeTo(0, duration);
                HealthValue.MoveTo(new Vector2(60, 10), duration, Easings.OutCubic);

                LeftValue.ClearTransforms();
                LeftValue.MoveTo(Vector2.Zero, duration, Easings.OutCubic);

                RightValue.ClearTransforms();
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

            private readonly MaskSprite health;
            private readonly MaskSprite energy;

            public CircularMask(Player player)
            {
                this.player = player;

                Scale = new Vector2(0.3f);

                Children = new[]
                {
                    health = new MaskSprite(Game.TextureStore.GetTexture("Gameplay\\health.png"))
                    {
                        Color = player.SecondaryColor
                    },
                    energy = new MaskSprite(Game.TextureStore.GetTexture("Gameplay\\energy.png"))
                    {
                        Color = player.ComplementaryColor
                    }
                };
            }

            private const float start = 0;
            private const float end = MathF.PI * 2;

            public override void Render()
            {
                Renderer.CircularProgram.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = Renderer.CircularProgram;
                Renderer.ShaderManager.UpdateFloat("startAngle",
                    PrionMath.Scale(player.Health, 0, player.HealthCapacity, end, start));
                Renderer.ShaderManager.UpdateFloat("endAngle", end);
                health.Render();
                Renderer.ShaderManager.UpdateFloat("startAngle", start);
                Renderer.ShaderManager.UpdateFloat("endAngle",
                    PrionMath.Scale(player.Energy, 0, player.EnergyCapacity, start, end));
                energy.Render();
                Renderer.SpriteProgram.SetActive();
            }
        }
    }
}