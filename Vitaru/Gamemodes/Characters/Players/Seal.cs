using System;
using System.Drawing;
using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Shaders;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.Transforms;
using Vitaru.Input;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class Seal : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(Seal);

        public Sprite Sign { get; private set; }
        public Sprite Reticle { get; private set; }

        private SpriteText rightValue;
        private SpriteText leftValue;

        private CircularMask circular;

        private Player player;

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
                leftValue = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,
                    TextScale = 0.25f,
                    //Color = player.SecondaryColor,
                },
                rightValue = new SpriteText
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,
                    TextScale = 0.25f,
                    //Color = player.SecondaryColor,
                },
            };
        }

        public void SpellActivate(VitaruActions action)
        {
            switch (player.Name)
            {
                case "Sakuya Izayoi":
                    Reticle.Color = Color.DarkRed;
                    Sign.Color = Color.DarkRed;
                    break;
            }
        }

        public void SpellDeactivate(VitaruActions action)
        {
            switch (player.Name)
            {
                case "Sakuya Izayoi":
                    Reticle.Color = player.PrimaryColor;
                    Sign.Color = player.PrimaryColor;
                    break;
            }
        }

        public void Update()
        {
            float speed = player.InputHandler.Actions[VitaruActions.Sneak] ? 1500 : 1000;

            if (!player.SpellActive)
                Sign.Rotation -= (float)(player.Clock.LastElapsedTime / speed);
            else
                Sign.Rotation += (float)(player.Clock.LastElapsedTime / speed);

            Reticle.Rotation =
                (float)Math.Atan2(player.Cursor.Y - player.Position.Y, player.Cursor.X - player.Position.X) +
                (float)Math.PI / 2f;

            Sign.Alpha = PrionMath.Scale(player.Energy, 0, player.EnergyCapacity);

            switch (player.Name)
            {
                case "Sakuya Izayoi":
                    Sakuya s = player as Sakuya;
                    leftValue.Text = s.SetRate.ToString();
                    break;
            }
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
                Reticle.FadeTo(Sign.Alpha, 200);
                Reticle.ScaleTo(new Vector2(0.2f), 200, Easings.OutCubic);
                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.2f), 200, Easings.OutCubic);
                circular.ClearTransforms();
                circular.ScaleTo(new Vector2(0.2f), 200, Easings.OutCubic);
            }
        }

        public void Released(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.ClearTransforms();
                Reticle.FadeTo(0f, 200);
                Reticle.ScaleTo(new Vector2(0.3f), 200, Easings.OutCubic);
                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.3f), 200, Easings.OutCubic);
                circular.ClearTransforms();
                circular.ScaleTo(new Vector2(0.3f), 200, Easings.OutCubic);
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
                        Color = player.ComplementaryColor,
                    },
                    energy = new MaskSprite(Game.TextureStore.GetTexture("Gameplay\\energy.png"))
                    {
                        Color = player.SecondaryColor,
                    },
                };
            }

            private const float start = (float)Math.PI / 2;
            private const float end = (float)Math.PI * 2;

            public override void Render()
            {
                Renderer.CircularProgram.SetActive();
                Renderer.ShaderManager.UpdateFloat(Renderer.CircularProgram, "startAngle", start);
                Renderer.ShaderManager.UpdateFloat(Renderer.CircularProgram, "endAngle", PrionMath.Scale(player.Health, 0, player.HealthCapacity, start, end));
                health.Render();
                Renderer.ShaderManager.UpdateFloat(Renderer.CircularProgram, "startAngle", start);
                Renderer.ShaderManager.UpdateFloat(Renderer.CircularProgram, "endAngle", PrionMath.Scale(player.Energy, 0, player.EnergyCapacity, start, end));
                energy.Render();
                Renderer.SpriteProgram.SetActive();
            }
        }
    }
}
