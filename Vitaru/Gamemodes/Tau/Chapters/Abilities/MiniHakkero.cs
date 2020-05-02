// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Tau.Chapters.Rational.Characters.Drawables;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Abilities
{
    public class MiniHakkero : Container
    {
        public readonly Hitbox Hitbox;

        private readonly Container red;
        private readonly Container green;
        private readonly Container blue;

        public MiniHakkero(DrawableMarisa drawableMarisa)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.BottomCentre;

            Children = new[]
            {
                Hitbox = new Hitbox(Shape.Rectangle)
                {
                    HitDetection = false
                },
                red = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,

                    Masking = true,
                    Colour = Color4.Red,
                    Alpha = 0.25f,

                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                },
                green = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,

                    Masking = true,
                    Colour = Color4.Green,
                    Alpha = 0.25f,

                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                },
                blue = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,

                    Masking = true,
                    Colour = Color4.Blue,
                    Alpha = 0.25f,

                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            };
        }

        public override void Show()
        {
            this.FadeIn(100);
        }

        public override void Hide()
        {
            this.FadeOut(100);
        }
    }
}