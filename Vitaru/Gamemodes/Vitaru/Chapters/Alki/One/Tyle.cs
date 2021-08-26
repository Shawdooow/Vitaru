// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Utilities;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Tyle : Player
    {
        #region Fields

        public const double CHARGE_TIME = 800;

        public const float BLINK_DISTANCE = 480;

        public override string Name => "Tyle";

        public override float HealthCapacity => 60;

        public override float EnergyCapacity => 16;

        public override float EnergyCost => 4;

        public override float EnergyDrainRate => 6;

        public override Color PrimaryColor => "#4903fc".HexToColor();

        public override Color SecondaryColor => "#262626".HexToColor();

        public override Color ComplementaryColor => "#8e70db".HexToColor();

        public override string Ability => "Shadow Blinker";

        public override Role Role => Role.Offense;

        public override Difficulty Difficulty => Difficulty.Extreme;

        public override bool Implemented => true;

        protected Sprite Landing;

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

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Gamefield.OverlaysLayer.Add(Landing = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            {
                Size = new Vector2(100),
                Alpha = 0,
                Color = ComplementaryColor
            });
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);
            spellStartTime = Gamefield.Current;

            Landing.ClearTransforms();
            Landing.FadeTo(1f, 200f, Easings.InCubic);
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (SpellActive)
            {
                charge = (float)Easing.ApplyEasing(Easings.OutSine, Math.Min(PrionMath.Remap(Gamefield.Current, spellStartTime, spellStartTime + CHARGE_TIME), 1));

                float cursorAngle = MathF.Atan2(InputManager.Mouse.Position.Y - Position.Y, InputManager.Mouse.Position.X - Position.X) + Drawable.Rotation;

                float dist = Math.Min(charge * BLINK_DISTANCE, Vector2.Distance(Position, InputManager.Mouse.Position));
                Vector2 blink = Position + PrionMath.Offset(dist, cursorAngle);

                Landing.Position = blink;

                DrainEnergy((float)Clock.LastElapsedTime / 1000 * EnergyDrainRate * charge);
            }

            if (Gamefield.Current >= spellEndTime)
                HitDetection = true;

            if (DrawablePlayer != null) DrawablePlayer.Seal.LeftValue.Text = $"{Math.Round(BLINK_DISTANCE * charge, 0)}p";
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            Landing.ClearTransforms();
            Landing.FadeTo(0, 200f, Easings.InCubic);

            HitDetection = false;
            spellEndTime = Gamefield.Current + 200 * charge;
            Drawable.Alpha = 0.25f;

            new Vector2Transform(value => Position = value, Position, Landing.Position, this, Gamefield.Current, 200 * charge, Easings.OutSine);
            new FloatTransform(value => Drawable.Alpha = value, Drawable.Alpha, 1, this, Gamefield.Current, 200 * charge, Easings.InCubic);

            charge = 0;
        }
    }
}