// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Worship.Characters.Drawables
{
    public class DrawableTomaji : DrawableTouhosuPlayer
    {
        /// <summary>
        ///     scale from 0 - 1 on how charged our blink is
        /// </summary>
        private double charge;

        private double spellStartTime = double.MaxValue;

        private double spellEndTime { get; set; } = double.MinValue;

        public DrawableTomaji(VitaruPlayfield playfield) : base(playfield, new Tomaji())
        {
        }

        protected override void SpellActivate(VitaruAction action)
        {
            base.SpellActivate(action);
            spellStartTime = VitaruPlayfield.Current;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (SpellActive)
            {
                double fullChargeTime = spellStartTime + Tomaji.CHARGE_TIME;

                charge = Math.Min(1 - (fullChargeTime - VitaruPlayfield.Current) / Tomaji.CHARGE_TIME, 1);

                Drain(Clock.ElapsedFrameTime / 1000 * TouhosuPlayer.EnergyDrainRate * charge);
            }

            if (VitaruPlayfield.Current >= spellEndTime)
                Hitbox.HitDetection = true;
        }

        protected override void SpellDeactivate(VitaruAction action)
        {
            base.SpellDeactivate(action);

            double cursorAngle =
                MathHelper.RadiansToDegrees(Math.Atan2(Cursor.Position.Y - Position.Y,
                    Cursor.Position.X - Position.X)) + Rotation - 12;
            double x = Position.X + charge * Tomaji.BLINK_DISTANCE * Math.Cos(MathHelper.DegreesToRadians(cursorAngle));
            double y = Position.Y + charge * Tomaji.BLINK_DISTANCE * Math.Sin(MathHelper.DegreesToRadians(cursorAngle));

            Hitbox.HitDetection = false;
            spellEndTime = VitaruPlayfield.Current + 200 * charge;
            Alpha = 0.25f;

            this.MoveTo(new Vector2((float) x, (float) y), 200 * charge, Easing.OutSine)
                .FadeIn(200 * charge, Easing.InCubic);

            charge = 0;
        }
    }
}