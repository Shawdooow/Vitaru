// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using Vitaru.Gamemodes.Tau.Chapters.Abilities;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Worship.Characters.Drawables
{
    public class DrawableRyukoy : DrawableTouhosuPlayer
    {
        private readonly Bindable<int> souls = VitaruSettings.VitaruConfigManager.GetBindable<int>(VitaruSetting.Souls);

        public override bool Untuned
        {
            get => untuned;
            set
            {
                if (value == untuned) return;

                untuned = value;


                if (value)
                {
                    VitaruPlayfield.Gamefield.Remove(this);
                    VitaruPlayfield.VitaruInputManager.BlurredPlayfield.Add(this);
                    CurrentPlayfield = VitaruPlayfield.VitaruInputManager.BlurredPlayfield;
                }
                else
                {
                    VitaruPlayfield.VitaruInputManager.BlurredPlayfield.Remove(this);
                    VitaruPlayfield.Gamefield.Add(this);
                    CurrentPlayfield = VitaruPlayfield.Gamefield;
                }

                if (VitaruPlayfield.Player == this)
                {
                    if (value)
                    {
                        VitaruPlayfield.RemoveDrawable(VitaruPlayfield.Gamefield);
                        VitaruPlayfield.VitaruInputManager.BlurContainer.Add(VitaruPlayfield.Gamefield);
                        VitaruPlayfield.Gamefield.Margin = 0.8f;
                        VitaruPlayfield.VitaruInputManager.BlurContainer.Remove(VitaruPlayfield.VitaruInputManager
                            .BlurredPlayfield);
                        VitaruPlayfield.AddDrawable(VitaruPlayfield.VitaruInputManager.BlurredPlayfield);
                        VitaruPlayfield.VitaruInputManager.BlurredPlayfield.Margin = 1f;
                    }
                    else
                    {
                        VitaruPlayfield.VitaruInputManager.BlurContainer.Remove(VitaruPlayfield.Gamefield);
                        VitaruPlayfield.AddDrawable(VitaruPlayfield.Gamefield);
                        VitaruPlayfield.Gamefield.Margin = 1f;
                        VitaruPlayfield.RemoveDrawable(VitaruPlayfield.VitaruInputManager.BlurredPlayfield);
                        VitaruPlayfield.VitaruInputManager.BlurContainer.Add(VitaruPlayfield.VitaruInputManager
                            .BlurredPlayfield);
                        VitaruPlayfield.VitaruInputManager.BlurredPlayfield.Margin = 0.8f;
                    }
                }
            }
        }

        private bool untuned;

        public DrawableRyukoy(VitaruPlayfield playfield) : base(playfield, new Ryukoy())
        {
            if (souls >= 10 && VitaruAPIContainer.Shawdooow)
                playfield.Gamefield.Add(new DrawableReimu(playfield, this)
                    {Ghost = true, ControlType = ControlType.Auto, Position = Position});

            if (souls < 100 && VitaruAPIContainer.Shawdooow)
                souls.ValueChanged += value =>
                {
                    switch (value)
                    {
                        case 1:
                            Speak("That felt. . . Strange. . .");
                            break;
                        case 5:
                            Speak("What are these. . ?");
                            break;
                        case 10:
                            Speak("I can feel\nsomeone familiar. . .");
                            playfield.Gamefield.Add(new DrawableReimu(playfield, this)
                                {Ghost = true, ControlType = ControlType.Auto, Position = Position});
                            break;
                        case 20:
                            Speak("I will just try to ignore it.");
                            break;
                        case 50:
                            Speak("I can almost hear. . .\nWhispers?");
                            break;
                        case 90:
                            Speak("They are getting louder.\nSo much louder!");
                            break;
                        case 100:
                            Speak("Mom!?");

                            Box box = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.White,
                                Alpha = 0
                            };
                            VitaruPlayfield.VitaruInputManager.Add(box);
                            box.FadeInFromZero(100, Easing.OutSine)
                                .FadeOutFromOne(900, Easing.InCubic)
                                .OnComplete(d =>
                                {
                                    VitaruPlayfield.VitaruInputManager.Remove(box);
                                    box.Expire();
                                });
                            break;
                    }
                };
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (SpellActive)
            {
                int players = 0;
                int enemies = 0;
                int bosses = 0;

                foreach (Drawable draw in VitaruPlayfield.VitaruInputManager.BlurredPlayfield)
                    switch (draw)
                    {
                        case DrawableVitaruPlayer player:
                            players++;
                            break;
                        case Enemy enemy:
                            enemies++;
                            break;
                        case DrawableBoss boss:
                            bosses++;
                            break;
                    }

                Drain(Clock.ElapsedFrameTime / 1000 * (players * Ryukoy.PLAYER_DRAIN_MULTIPLIER +
                                                       enemies * Ryukoy.ENEMY_DRAIN_MULTIPLIER +
                                                       bosses * Ryukoy.BOSS_DRAIN_MULTIPLIER));
            }
            else if (Untuned)
                Untuned = false;
        }

        protected override bool CheckSpellActivate(VitaruAction action)
        {
            if (action == VitaruAction.Pull) return true;
            return base.CheckSpellActivate(action);
        }

        protected override void SpellActivate(VitaruAction action)
        {
            if (action == VitaruAction.Pull)
            {
                restart:
                foreach (Drawable draw in !Untuned
                    ? VitaruPlayfield.Gamefield
                    : VitaruPlayfield.VitaruInputManager.BlurredPlayfield)
                    if (draw is ITuneable tunable && draw.Alpha > 0)
                    {
                        Vector2 drawPos = Cursor.ToSpaceOfOtherDrawable(Vector2.Zero, draw);
                        float distance = (float) Math.Sqrt(Math.Pow(drawPos.X, 2) + Math.Pow(drawPos.Y, 2));

                        if (80 >= distance && Energy >= Ryukoy.BULLET_ENERGY_COST)
                        {
                            Drain(Ryukoy.BULLET_ENERGY_COST);
                            tunable.Untuned = !tunable.Untuned;
                            goto restart;
                        }
                    }

                return;
            }

            base.SpellActivate(action);
            Untuned = true;
            //LeftTotem.Alpha = 1;
            //RightTotem.Alpha = 1;
        }

        protected override void SpellDeactivate(VitaruAction action)
        {
            base.SpellDeactivate(action);
            Untuned = false;
            //LeftTotem.Alpha = 0;
            //RightTotem.Alpha = 0;
        }
    }
}