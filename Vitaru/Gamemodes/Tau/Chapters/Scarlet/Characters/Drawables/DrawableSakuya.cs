// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters.Drawables
{
    public class DrawableSakuya : DrawableTouhosuPlayer
    {
        #region Fields

        protected AnimatedSprite Idle;
        protected AnimatedSprite Left;
        protected AnimatedSprite Right;

        public double SetRate { get; private set; } = 0.75d;

        private double originalRate;

        private double currentRate = 1;

        private readonly Bindable<WorkingBeatmap> workingBeatmap = new Bindable<WorkingBeatmap>();

        private double spellEndTime { get; set; } = double.MinValue;

        #endregion

        public DrawableSakuya(VitaruPlayfield playfield) : base(playfield, new Sakuya())
        {
        }

        protected override void LoadAnimationSprites(TextureStore textures, Storage storage)
        {
            if (PlayerVisuals == GraphicsOptions.Old)
                base.LoadAnimationSprites(textures, storage);
            else
            {
                SoulContainer.Alpha = 0;
                KiaiContainer.Alpha = 1;

                KiaiLeftSprite.Alpha = 0;
                KiaiRightSprite.Alpha = 0;
                KiaiStillSprite.Alpha = 0;

                KiaiContainer.AddRange(new[]
                {
                    Idle = new AnimatedSprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        UpdateRate = 100,
                        Textures = new[]
                        {
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 0", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 1", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 2", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 3", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 4", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 5", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 6", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai 7", storage)
                        }
                    },
                    Left = new AnimatedSprite
                    {
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both,
                        UpdateRate = 100,
                        Textures = new[]
                        {
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 0", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 1", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 2", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 3", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 4", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 5", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 6", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Left 7", storage)
                        }
                    },
                    Right = new AnimatedSprite
                    {
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both,
                        UpdateRate = 100,
                        Textures = new[]
                        {
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 0", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 1", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 2", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 3", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 4", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 5", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 6", storage),
                            VitaruSkinElement.LoadSkinElement(Player.Name + " Kiai Right 7", storage)
                        }
                    }
                });
            }
        }

        protected override void MovementAnimations()
        {
            if (PlayerVisuals == GraphicsOptions.Old)
                base.MovementAnimations();
            else
            {
                if (Position.X > LastX && Right.Alpha < 1)
                {
                    Idle.Alpha = 0;
                    Left.Alpha = 0;
                    Right.Alpha = 1;
                    Right.Reset();
                }
                else if (Position.X < LastX && Left.Alpha < 1)
                {
                    Idle.Alpha = 0;
                    Left.Alpha = 1;
                    Right.Alpha = 0;
                    Left.Reset();
                }
                else if (Position.X == LastX && Idle.Alpha < 1)
                {
                    Idle.Alpha = 1;
                    Left.Alpha = 0;
                    Right.Alpha = 0;
                    Idle.Reset();
                }

                LastX = Position.X;
            }
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<WorkingBeatmap> beatmap)
        {
            workingBeatmap.BindTo(beatmap);
        }

        protected override bool CheckSpellActivate(VitaruAction action)
        {
            if (action == VitaruAction.Increase)
                return true;
            if (action == VitaruAction.Decrease)
                return true;

            return base.CheckSpellActivate(action);
        }

        protected override void SpellActivate(VitaruAction action)
        {
            if (action == VitaruAction.Increase)
            {
                SetRate = Math.Min(
                    Actions[VitaruAction.Slow] ? Math.Round(SetRate + 0.05d, 2) : Math.Round(SetRate + 0.25d, 2), 2d);
                return;
            }

            if (action == VitaruAction.Decrease)
            {
                SetRate = Math.Max(
                    Actions[VitaruAction.Slow] ? Math.Round(SetRate - 0.05d, 2) : Math.Round(SetRate - 0.25d, 2), -2d);
                return;
            }

            base.SpellActivate(action);

            if (originalRate == 0)
                originalRate = (float) workingBeatmap.Value.Track.Rate;

            currentRate = originalRate * SetRate;
            applyToClock(workingBeatmap.Value.Track, currentRate);

            Seal.SignSprite.Colour = Color4.DarkRed;

            if (currentRate > 0)
                spellEndTime = VitaruPlayfield.Current + 2000;
            else if (currentRate == 0)
                spellEndTime = VitaruPlayfield.Current;
            else
                spellEndTime = VitaruPlayfield.Current - 2000;
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (spellEndTime >= VitaruPlayfield.Current && currentRate > 0 ||
                spellEndTime == VitaruPlayfield.Current && currentRate == 0 ||
                spellEndTime <= VitaruPlayfield.Current && currentRate < 0)
                if (!SpellActive)
                {
                    currentRate += (float) Clock.ElapsedFrameTime / 100;

                    if (currentRate > originalRate || currentRate <= 0)
                        currentRate = originalRate;

                    applyToClock(workingBeatmap.Value.Track, currentRate);

                    if (currentRate > 0 && spellEndTime - 500 <= VitaruPlayfield.Current)
                    {
                        currentRate = originalRate;
                        applyToClock(workingBeatmap.Value.Track, currentRate);
                    }
                    else if (currentRate < 0 && spellEndTime + 500 >= VitaruPlayfield.Current)
                    {
                        currentRate = originalRate;
                        applyToClock(workingBeatmap.Value.Track, currentRate);
                    }
                }
                else
                {
                    double energyDrainMultiplier = 0;

                    if (currentRate < 1)
                        energyDrainMultiplier = originalRate - currentRate;
                    else if (currentRate >= 1)
                        energyDrainMultiplier = currentRate - originalRate;

                    Drain(Clock.ElapsedFrameTime / 1000 *
                          (1 / currentRate * energyDrainMultiplier * TouhosuPlayer.EnergyDrainRate +
                           TouhosuPlayer.EnergyCost));

                    if (currentRate > 0)
                        spellEndTime = VitaruPlayfield.Current + 2000;
                    else if (currentRate == 0)
                        spellEndTime = VitaruPlayfield.Current;
                    else
                        spellEndTime = VitaruPlayfield.Current - 2000;

                    currentRate = originalRate * SetRate;
                    applyToClock(workingBeatmap.Value.Track, currentRate);
                }
        }

        protected override void SpellDeactivate(VitaruAction action)
        {
            base.SpellDeactivate(action);
            Seal.SignSprite.FadeColour(Player.PrimaryColor, 50);
        }

        private void applyToClock(IAdjustableClock clock, double speed)
        {
            if (VitaruPlayfield.VitaruInputManager.Shade != null)
            {
                if (speed > 1)
                {
                    VitaruPlayfield.VitaruInputManager.Shade.Colour = Color4.Cyan;
                    VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float) (speed - 1) * 0.05f;
                }
                else if (speed == 1)
                    VitaruPlayfield.VitaruInputManager.Shade.Alpha = 0;
                else if (speed < 1 && speed > 0)
                {
                    VitaruPlayfield.VitaruInputManager.Shade.Colour = Color4.Orange;
                    VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float) (1 - speed) * 0.05f;
                }
                else if (speed < 0)
                {
                    VitaruPlayfield.VitaruInputManager.Shade.Colour = Color4.Purple;
                    VitaruPlayfield.VitaruInputManager.Shade.Alpha = (float) -speed * 0.1f;
                }
            }

            if (clock is IHasPitchAdjust pitchAdjust)
                pitchAdjust.PitchAdjust = speed;

            SpeedMultiplier = 1 / speed;
        }

        protected override void Dispose(bool isDisposing)
        {
            workingBeatmap.UnbindAll();
            base.Dispose(isDisposing);
        }
    }
}