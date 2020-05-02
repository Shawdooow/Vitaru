// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Application.Debug;
using Prion.Application.Timing;
using Prion.Application.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters.Drawables;
using Vitaru.Input;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters
{
    public class Sakuya : Player
    {
        #region Fields

        public override string Name => "Sakuya Izayoi";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 24;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 4;

        public override Color PrimaryColor => Color.Navy;

        public override Color SecondaryColor => "#92a0dd".HexToColor();

        public override Color ComplementaryColor => "#d6d6d6".HexToColor();

        public double SetRate { get; private set; } = 0.75d;

        private double originalRate;

        private double currentRate = 1;

        private double spellEndTime { get; set; } = double.MinValue;

        #endregion

        public override string Ability => "Time Waster";

        public override Role Role => Role.Defense;

        public override Difficulty Difficulty => Difficulty.Normal;

        public override string Background =>
            "      Sakuya is no stranger to the oddities in the world, but never could they stop her from besting her opponents. " +
            "Her perfect record has only been tainted by one person, but The Hakureis are close friends of hers now.\n\n" +
            "       They have put there differences aside once to fight off something bigger then all of them combined, " +
            "but as the phrase goes: \"Greater than the sum of its parts\" they were able to hold the fort long enough to succeed.";

        public override bool Implemented => false;

        private AdjustableClock adjustable;

        public override DrawablePlayer GenerateDrawable()
        {
            DrawableSakuya draw = new DrawableSakuya(this)
            {
                Position = new Vector2(0, 200)
            };
            Drawable = draw;
            return draw;
        }

        public Sakuya(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            if (Clock is AdjustableClock a)
                adjustable = a;
            else
                PrionDebugger.InvalidOperation($"{nameof(Sakuya)} requires an {nameof(AdjustableClock)}!");
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
                    InputHandler.Actions[VitaruActions.Sneak]
                        ? Math.Round(SetRate + 0.05d, 2)
                        : Math.Round(SetRate + 0.25d, 2), 2d);
                return;
            }

            if (action == VitaruActions.Decrease)
            {
                SetRate = Math.Max(
                    InputHandler.Actions[VitaruActions.Sneak]
                        ? Math.Round(SetRate - 0.05d, 2)
                        : Math.Round(SetRate - 0.25d, 2), -2d);
                return;
            }

            base.SpellActivate(action);

            if (originalRate == 0)
                originalRate = (float) ((AdjustableClock) Clock).Rate;

            currentRate = originalRate * SetRate;
            applyToClock(adjustable, currentRate);

            DrawablePlayer.SignSprite.Color = Color.DarkRed;

            if (currentRate > 0)
                spellEndTime = Clock.Current + 2000;
            else if (currentRate == 0)
                spellEndTime = Clock.Current;
            else
                spellEndTime = Clock.Current - 2000;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (spellEndTime >= Clock.Current && currentRate > 0 ||
                spellEndTime == Clock.Current && currentRate == 0 ||
                spellEndTime <= Clock.Current && currentRate < 0)
                if (!SpellActive)
                {
                    currentRate += (float) Clock.ElapsedTime / 100;

                    if (currentRate > originalRate || currentRate <= 0)
                        currentRate = originalRate;

                    applyToClock(adjustable, currentRate);

                    if (currentRate > 0 && spellEndTime - 500 <= Clock.Current)
                    {
                        currentRate = originalRate;
                        applyToClock(adjustable, currentRate);
                    }
                    else if (currentRate < 0 && spellEndTime + 500 >= Clock.Current)
                    {
                        currentRate = originalRate;
                        applyToClock(adjustable, currentRate);
                    }
                }
                else
                {
                    double energyDrainMultiplier = 0;

                    if (currentRate < 1)
                        energyDrainMultiplier = originalRate - currentRate;
                    else if (currentRate >= 1)
                        energyDrainMultiplier = currentRate - originalRate;

                    DrainEnergy((float) Clock.ElapsedTime / 1000f *
                                (1f / (float) currentRate * (float) energyDrainMultiplier * EnergyDrainRate +
                                 EnergyCost));

                    if (currentRate > 0)
                        spellEndTime = Clock.Current + 2000;
                    else if (currentRate == 0)
                        spellEndTime = Clock.Current;
                    else
                        spellEndTime = Clock.Current - 2000;

                    currentRate = originalRate * SetRate;
                    applyToClock(adjustable, currentRate);
                }
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);
            DrawablePlayer.SignSprite.Color = PrimaryColor;
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

            //if (clock is IHasPitchAdjust pitchAdjust)
            //    pitchAdjust.PitchAdjust = speed;
            clock.Rate = speed;

            MovementSpeedMultiplier = 1 / speed;
        }
    }
}