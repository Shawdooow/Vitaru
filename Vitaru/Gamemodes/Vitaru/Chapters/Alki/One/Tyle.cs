// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Utilities;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Input;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Tyle : Player
    {
        #region Fields

        public const double CHARGE_TIME = 1000;

        public const double BLINK_DISTANCE = 320;

        public override string Name => "Tyle";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 12;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 8;

        public override Color PrimaryColor => "#4903fc".HexToColor();

        public override Color SecondaryColor => "#262626".HexToColor();

        public override Color ComplementaryColor => "#8e70db".HexToColor();

        public override string Ability => "Shadow Skipper";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override bool Implemented => true;

        /// <summary>
        /// scale from 0 - 1 on how charged our blink is
        /// </summary>
        private float charge;

        private double spellStartTime = double.MaxValue;

        private double spellEndTime { get; set; } = double.MinValue;


        #endregion

        public Tyle(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);
            spellStartTime = Gamefield.Current;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (SpellActive)
            {
                charge = (float)Math.Min(PrionMath.Remap(Gamefield.Current, spellStartTime, spellStartTime + CHARGE_TIME), 1);

                DrainEnergy((float)Clock.LastElapsedTime / 1000 * EnergyDrainRate * charge);
            }

            if (Gamefield.Current >= spellEndTime)
                HitDetection = true;

            if (DrawablePlayer != null) DrawablePlayer.Seal.LeftValue.Text = $"{Math.Round(BLINK_DISTANCE * charge, 0)}p";
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            double cursorAngle = Math.Atan2(InputManager.Mouse.Position.Y - Position.Y, InputManager.Mouse.Position.X - Position.X).ToDegrees() + Drawable.Rotation;
            double x = Position.X + charge * BLINK_DISTANCE * Math.Cos(cursorAngle.ToRadians());
            double y = Position.Y + charge * BLINK_DISTANCE * Math.Sin(cursorAngle.ToRadians());

            HitDetection = false;
            spellEndTime = Gamefield.Current + 200 * charge;
            Drawable.Alpha = 0.25f;

            new Vector2Transform(value => Position = value, Position, new Vector2((float)x, (float)y), this, Gamefield.Current, 200 * charge, Easings.OutSine);
            new FloatTransform(value => Drawable.Alpha = value, Drawable.Alpha, 1, this, Gamefield.Current, 200 * charge, Easings.InCubic);

            charge = 0;
        }
    }
}