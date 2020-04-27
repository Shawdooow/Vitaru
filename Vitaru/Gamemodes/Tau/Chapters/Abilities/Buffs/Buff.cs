// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Abilities.Buffs
{
    public class Buff : Container, ITuneable
    {
        public AspectLockedPlayfield CurrentPlayfield { get; set; }

        private readonly ChapterSet chapterSet =
            ChapterStore.GetChapterSet(VitaruSettings.VitaruConfigManager.Get<string>(VitaruSetting.Gamemode));

        public virtual bool Untuned
        {
            get => untuned;
            set
            {
                if (value == untuned) return;

                untuned = value;

                if (value)
                {
                    playfield.Gamefield.Remove(this);
                    playfield.VitaruInputManager.BlurredPlayfield.Add(this);
                    CurrentPlayfield = playfield.VitaruInputManager.BlurredPlayfield;
                }
                else
                {
                    playfield.VitaruInputManager.BlurredPlayfield.Remove(this);
                    playfield.Gamefield.Add(this);
                    CurrentPlayfield = playfield.Gamefield;
                }
            }
        }

        private bool untuned;

        private readonly VitaruPlayfield playfield;

        public Buff(VitaruPlayfield playfield)
        {
            this.playfield = playfield;
            CurrentPlayfield = playfield.Gamefield;

            AlwaysPresent = true;
            Masking = true;
            Size = new Vector2(12, 18);
            CornerRadius = 4;
            BorderThickness = 2;
            BorderColour = Color4.White;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Red
            };
            EdgeEffect = new EdgeEffectParameters
            {
                Colour = Color4.Yellow.Opacity(0.5f),
                Type = EdgeEffectType.Shadow,
                Radius = Size.X * 2
            };
        }

        private bool killed;

        protected override void Update()
        {
            base.Update();

            if (Position.Y >= chapterSet.PlayfieldBounds.W + 10 && !killed)
            {
                killed = true;
                this.FadeOut(100)
                    .OnComplete(b => { Expire(); });
            }
            else if (!killed)
            {
                double distance = Math.Sqrt(Math.Pow(playfield.MousePos.X, 2) + Math.Pow(playfield.MousePos.Y, 2));
                Alpha = (float) GetAlpha(distance);
            }
        }

        protected double GetAlpha(double distance)
        {
            const double alpha_max = 0.6d;
            const double alpha_min = 0.05d;
            const double range = 160;
            const double scale = (alpha_max - alpha_min) / (0 - range);

            return Math.Max(alpha_min + (distance - range) * scale, alpha_min);
        }
    }
}