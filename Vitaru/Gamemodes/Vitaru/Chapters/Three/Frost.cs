// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Timing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Graphics;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Tracks;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Three
{
    public class Frost : Player
    {
        #region Fields

        public override string Name => "Frost";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 24;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 4;

        public override Color PrimaryColor => "#a1e4ff".HexToColor();

        public override Color SecondaryColor => "#009ad9".HexToColor();

        public override Color ComplementaryColor => "#c2c2c2".HexToColor();

        public double SetRate { get; private set; } = 0.75d;

        private double originalRate;

        private double currentRate = 1;

        private double spellEndTime { get; set; } = double.MinValue;

        #endregion

        public override string Ability => "Time Traveler";

        public override Role Role => Role.Defense;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override string Background => "";

        private AdjustableClock adjustable;

        public Frost(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            if (Clock is AdjustableClock a)
                adjustable = a;
            else
                Debugger.InvalidOperation($"{nameof(Frost)} requires an {nameof(AdjustableClock)}!");
        }

        protected override bool CheckSpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.Increase)
                return true;
            if (action == VitaruActions.Decrease)
                return true;

            return base.CheckSpellActivate(action);
        }

        protected override void SpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.Increase)
            {
                SetRate = Math.Min(
                    Binds[VitaruActions.Sneak]
                        ? Math.Round(SetRate + 0.05d, 2)
                        : Math.Round(SetRate + 0.25d, 2), 2d);
                intensity();
                return;
            }

            if (action == VitaruActions.Decrease)
            {
                SetRate = Math.Max(
                    Binds[VitaruActions.Sneak]
                        ? Math.Round(SetRate - 0.05d, 2)
                        : Math.Round(SetRate - 0.25d, 2), -2d);
                intensity();
                return;
            }

            base.SpellActivate(action);

            if (originalRate == 0)
                originalRate = (float) adjustable.Rate;

            currentRate = originalRate * SetRate;
            applyToClock(adjustable, currentRate);

            if (currentRate > 0)
                spellEndTime = Clock.LastCurrent + 2000;
            else if (currentRate == 0)
                spellEndTime = Clock.LastCurrent;
            else
                spellEndTime = Clock.LastCurrent - 2000;

            intensity();
            Gamefield.Shade = Shades.Blue;
            Gamefield.CharacterLayer.Shade = Shades.Blue;
            //Gamefield.BulletLayer.Shade = Shades.Blue;
            DrawablePlayer.Sprite.Color = Color.Blue;
            DrawablePlayer.HitboxOutline.Color = Color.Blue;
            DrawablePlayer.Seal.Reticle.Color = Color.GhostWhite;
            DrawablePlayer.Seal.Sign.Color = Color.GhostWhite;
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
            float scale = (float) Easing.ApplyEasing(Easings.OutQuad,
                Math.Min(PrionMath.Scale(currentRate, 1d, currentRate > 1d ? 2d : 0.5d), 1d));
            Gamefield.Intensity = scale;
            Gamefield.CharacterLayer.Intensity = scale;
            //Gamefield.BulletLayer.Intensity = scale;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (spellEndTime >= Clock.LastCurrent && currentRate > 0 ||
                spellEndTime == Clock.LastCurrent && currentRate == 0 ||
                spellEndTime <= Clock.LastCurrent && currentRate < 0)
                if (!SpellActive)
                {
                    currentRate += (float) Clock.LastElapsedTime / 200;

                    if (currentRate > originalRate || currentRate <= 0)
                        currentRate = originalRate;

                    applyToClock(adjustable, currentRate);

                    if (currentRate > 0 && spellEndTime - 1000 <= Clock.LastCurrent ||
                        currentRate < 0 && spellEndTime + 1000 >= Clock.LastCurrent)
                    {
                        currentRate = originalRate;
                        applyToClock(adjustable, currentRate);

                        Gamefield.Shade = Shades.None;
                        Gamefield.CharacterLayer.Shade = Shades.None;
                        //Gamefield.BulletLayer.Shade = Shades.None;
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

                    DrainEnergy((float) Clock.LastElapsedTime / 1000f *
                                (1f / (float) currentRate * (float) energyDrainMultiplier * EnergyDrainRate +
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
            //if (VitaruPlayfield.VitaruInputManager.Shade != null)
            //{
            //    if (speed > 1)
            //    {
            //        VitaruPlayfield.VitaruInputManager.Shade.Colour = Color.Cyan;
            //        VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float)(speed - 1) * 0.05f;
            //    }
            //    else if (speed == 1)
            //        VitaruPlayfield.VitaruInputManager.Shade.Alpha = 0;
            //    else if (speed < 1 && speed > 0)
            //    {
            //        VitaruPlayfield.VitaruInputManager.Shade.Colour = Color.Orange;
            //        VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float)(1 - speed) * 0.05f;
            //    }
            //    else if (speed < 0)
            //    {
            //        VitaruPlayfield.VitaruInputManager.Shade.Colour = Color.Purple;
            //        VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float)-speed * 0.1f;
            //    }
            //}

            if (TrackManager.CurrentTrack != null)
                TrackManager.CurrentTrack.Pitch = (float) speed;
            clock.Rate = speed;

            MovementSpeedMultiplier = 1 / speed;
        }

        protected override void BulletAddRad(float speed, float angle, Color color, float size, float damage,
            float distance)
        {
            if (SpellActive && color == PrimaryColor) color = Color.Blue;
            base.BulletAddRad(speed, angle, color, size, damage, distance);
        }
    }
}