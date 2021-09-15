// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using Prion.Golgi.Audio.Tracks;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two
{
    public class Yuie : Player
    {
        #region Fields


        public override string Name => "Yuie";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 16;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 4;

        public override Color PrimaryColor => "#00ffbb".HexToColor();

        public override Color SecondaryColor => "#b0b0b0".HexToColor();

        public override Color ComplementaryColor => "#3d2a69".HexToColor();

        public override string Seal => "Gameplay\\seal alki 2.png";

        public override float SealRotationSpeed => -0.5f;

        public override string Ability => "Time Dilator";

        public override Role Role => Role.Defense;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override string Notes =>
            $"Energy Drain Rate scales with absolute difference between Speed Multiplier ({nameof(SetRate)})x and 1 + Energy Cost ({EnergyCost}SP)";

        public override bool Implemented => true;

        public double SetRate { get; private set; } = 0.75d;

        private double originalRate;

        private double currentRate = 1;

        private double spellEndTime { get; set; } = double.MinValue;

        private AdjustableClock adjustable;


        #endregion


        public Yuie(Gamefield gamefield) : base(gamefield) { }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            if (Clock is AdjustableClock a)
                adjustable = a;
            else
                Debugger.InvalidOperation(
                    $"{nameof(Yuie)} requires an {nameof(AdjustableClock)} as it's {nameof(Clock)}!");
        }

        protected override bool CheckSpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.ModifierOne)
                return true;
            if (action == VitaruActions.ModifierTwo)
                return true;

            return base.CheckSpellActivate(action);
        }

        protected override void SpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.ModifierOne)
            {
                SetRate = Math.Min(
                    GetBind(VitaruActions.Sneak)
                        ? Math.Round(SetRate + 0.05d, 2)
                        : Math.Round(SetRate + 0.25d, 2), 2d);
                intensity();
                Gamefield.Shade = SetRate > 1 ? Shades.Blue : Shades.Red;
                return;
            }

            if (action == VitaruActions.ModifierTwo)
            {
                SetRate = Math.Max(
                    GetBind(VitaruActions.Sneak)
                        ? Math.Round(SetRate - 0.05d, 2)
                        : Math.Round(SetRate - 0.25d, 2), -2d);
                intensity();
                Gamefield.Shade = SetRate > 1 ? Shades.Blue : Shades.Red;
                return;
            }

            base.SpellActivate(action);

            if (originalRate == 0)
                originalRate = (float)adjustable.Rate;

            currentRate = originalRate * SetRate;
            applyToClock(adjustable, currentRate);

            if (currentRate > 0)
                spellEndTime = Clock.LastCurrent + 2000;
            else if (currentRate == 0)
                spellEndTime = Clock.LastCurrent;
            else
                spellEndTime = Clock.LastCurrent - 2000;

            intensity();
            Gamefield.Shade = SetRate > 1 ? Shades.Blue : Shades.Red;
            DrawablePlayer.Sprite.Color = ComplementaryColor;
            DrawablePlayer.HitboxOutline.Color = ComplementaryColor;
            DrawablePlayer.Seal.Reticle.Color = SecondaryColor;
            DrawablePlayer.Seal.Sign.Color = SecondaryColor;
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            DrawablePlayer.Sprite.Color = PrimaryColor;
            DrawablePlayer.HitboxOutline.Color = PrimaryColor;
            DrawablePlayer.Seal.Reticle.Color = PrimaryColor;
            DrawablePlayer.Seal.Sign.Color = PrimaryColor;
        }

        private void intensity()
        {
            float scale = (float)Easing.ApplyEasing(Easings.OutQuad,
                Math.Min(PrionMath.Remap(currentRate, 1d, currentRate > 1d ? 2d : 0.5d), 1d));
            Gamefield.Intensity = scale;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (spellEndTime >= Clock.LastCurrent && currentRate > 0 ||
                spellEndTime == Clock.LastCurrent && currentRate == 0 ||
                spellEndTime <= Clock.LastCurrent && currentRate < 0)
                if (!SpellActive)
                {
                    currentRate += (float)Clock.LastElapsedTime / 200;

                    if (currentRate > originalRate || currentRate <= 0)
                        currentRate = originalRate;

                    applyToClock(adjustable, currentRate);

                    if (currentRate > 0 && spellEndTime - 1000 <= Clock.LastCurrent ||
                        currentRate < 0 && spellEndTime + 1000 >= Clock.LastCurrent)
                    {
                        currentRate = originalRate;
                        applyToClock(adjustable, currentRate);

                        Gamefield.Shade = Shades.None;
                    }

                    intensity();
                }
                else
                {
                    double energyDrainMultiplier = 0;

                    if (currentRate < 1)
                        energyDrainMultiplier = originalRate - currentRate;
                    else if (currentRate >= 1)
                        energyDrainMultiplier = currentRate - originalRate;

                    DrainEnergy((float)Clock.LastElapsedTime / 1000f *
                                (1f / (float)currentRate * (float)energyDrainMultiplier * EnergyDrainRate +
                                 EnergyCost));

                    if (currentRate > 0)
                        spellEndTime = Clock.LastCurrent + 2000;
                    else if (currentRate == 0)
                        spellEndTime = Clock.LastCurrent;
                    else
                        spellEndTime = Clock.LastCurrent - 2000;

                    currentRate = originalRate * SetRate;
                    applyToClock(adjustable, currentRate);
                    intensity();
                }

            if (DrawablePlayer != null) DrawablePlayer.Seal.LeftValue.Text = $"{SetRate}x";
        }

        private void applyToClock(AdjustableClock clock, double speed)
        {
            if (TrackManager.CurrentTrack != null)
                TrackManager.CurrentTrack.Pitch = (float)speed;
            clock.Rate = speed;

            MovementSpeedMultiplier = 1 / speed;
        }

        protected override void BulletAddRad(float speed, float angle, Color color, float size, float damage,
            float distance)
        {
            if (SpellActive && color == PrimaryColor) color = ComplementaryColor;
            base.BulletAddRad(speed, angle, color, size, damage, distance);
        }
    }
}