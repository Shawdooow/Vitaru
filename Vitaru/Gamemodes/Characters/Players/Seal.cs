using System;
using System.Drawing;
using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game;
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
        public Sprite Sign { get; private set; }
        public Sprite Reticle { get; private set; }

        //private SpriteText rightValue;
        //private SpriteText leftValue;

        //private CircularProgress health;
        //private CircularProgress energy;

        private Player player;

        public Seal(Player player)
        {
            this.player = player;

            Children = new[]
            {
                Sign = new Sprite(Game.TextureStore.GetTexture("Gameplay\\seal.png"))
                {
                    Scale = new Vector2(0.3f),
                    Alpha = 0.5f,
                    Color = player.PrimaryColor
                },
                Reticle = new Sprite(Game.TextureStore.GetTexture("Gameplay\\reticle.png"))
                {
                    Scale = new Vector2(0.5f),
                    Alpha = 0f,
                    Color = player.SecondaryColor
                },
            };
        }

        public void SpellActivate(VitaruActions action)
        {
            switch (player.Name)
            {
                case "Sakuya Izayoi":
                    Sign.Color = Color.DarkRed;
                    break;
            }
        }

        public void SpellDeactivate(VitaruActions action)
        {
            switch (player.Name)
            {
                case "Sakuya Izayoi":
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

            //TODO: this UI
            //energy.Current.Value = player.Energy / player.EnergyCapacity;
            //health.Current.Value = player.Health / player.HealthCapacity;

            switch (player.Name)
            {
                case "Sakuya Izayoi":
                    Sakuya s = player as Sakuya;
                    //leftValue.Text = s.SetRate.ToString();
                    break;
            }
        }

        public void Shoot(double flash)
        {
            if (player.InputHandler.Actions[VitaruActions.Sneak])
            {
                Reticle.Alpha = 1f;
                Reticle.FadeTo(0.75f, flash, Easings.OutCubic);
            }
        }

        public void Pressed(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.ClearTransforms();
                Reticle.FadeTo(0.75f, 200);
                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.2f), 200, Easings.OutCubic);
            }
        }

        public void Released(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Reticle.ClearTransforms();
                Reticle.FadeTo(0f, 200);
                Sign.ClearTransforms();
                Sign.ScaleTo(new Vector2(0.3f), 200, Easings.OutCubic);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            player = null;
            base.Dispose(isDisposing);
        }
    }
}
