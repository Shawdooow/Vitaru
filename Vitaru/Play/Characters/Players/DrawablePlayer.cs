// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Groups;
using Prion.Nucleus.Utilities;
using Vitaru.Input;

namespace Vitaru.Play.Characters.Players
{
    public class DrawablePlayer : DrawableCharacter
    {
        public override string Name { get; set; } = nameof(DrawablePlayer);

        protected Seal Seal;

        private const double duration = 200;

        public DrawablePlayer(Layer2D<IDrawable2D> layer) : base(layer)
        {
            CharacterLayer.Add(Seal = new Seal(), AddPosition.First);
        }

        public virtual void UpdateAnimations(Player player)
        {
            float amount = player.GetBind(VitaruActions.Sneak) ? 1500 : 1000;

            if (!player.SpellActive)
                Seal.Sign.Rotation += (float)(player.Clock.LastElapsedTime / amount * player.SealRotationSpeed);
            else
                Seal.Sign.Rotation -= (float)(player.Clock.LastElapsedTime / amount * player.SealRotationSpeed);

            Seal.EnergyValue.Text = $"{Math.Round(player.Energy, 0)}SP";
            Seal.HealthValue.Text = $"{Math.Round(player.Health, 0)}HP";

            Seal.Sign.Alpha = PrionMath.Remap(player.Energy, 0, player.EnergyCapacity, 0.1f);
        }

        public void Pressed(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Seal.Sign.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                Seal.Circular.ScaleTo(new Vector2(0.2f), duration, Easings.OutCubic);

                Seal.EnergyValue.FadeTo(0.8f, duration);
                Seal.EnergyValue.MoveTo(new Vector2(-40, 40), duration, Easings.OutCubic);

                Seal.HealthValue.FadeTo(0.8f, duration);
                Seal.HealthValue.MoveTo(new Vector2(40, 40), duration, Easings.OutCubic);

                Seal.LeftValue.MoveTo(new Vector2(40, 0), duration, Easings.OutCubic);
                Seal.RightValue.MoveTo(new Vector2(-40, 0), duration, Easings.OutCubic);
            }
        }

        public void Released(VitaruActions action)
        {
            if (action == VitaruActions.Sneak)
            {
                Seal.Sign.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                Seal.Circular.ScaleTo(new Vector2(0.3f), duration, Easings.OutCubic);

                Seal.EnergyValue.FadeTo(0, duration);
                Seal.EnergyValue.MoveTo(new Vector2(-60, 10), duration, Easings.OutCubic);

                Seal.HealthValue.FadeTo(0, duration);
                Seal.HealthValue.MoveTo(new Vector2(60, 10), duration, Easings.OutCubic);

                Seal.LeftValue.MoveTo(Vector2.Zero, duration, Easings.OutCubic);
                Seal.RightValue.MoveTo(Vector2.Zero, duration, Easings.OutCubic);
            }
        }
    }
}