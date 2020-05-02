// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using System.Collections.Generic;
using Vitaru.Gamemodes.Tau.Chapters.Abilities.Buffs;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Worship.Characters.Drawables
{
    public class DrawableReimu : DrawableTouhosuPlayer
    {
        private readonly Bindable<int> souls = VitaruSettings.VitaruConfigManager.GetBindable<int>(VitaruSetting.Souls);

        private readonly List<DrawableProjectile> projectiles = new List<DrawableProjectile>();

        public bool Ghost;

        public DrawableReimu(VitaruPlayfield playfield) : base(playfield, new Reimu())
        {
        }

        private readonly DrawableRyukoy ryukoy;

        public DrawableReimu(VitaruPlayfield playfield, DrawableRyukoy ryukoy) : base(playfield, new Reimu())
        {
            this.ryukoy = ryukoy;
            AlwaysPresent = true;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (Ghost)
            {
                Untuned = true;
                SoulContainer.Colour = Color4.Cyan;
                KiaiContainer.Colour = Color4.Cyan;
                Seal.Sign.Colour = Color4.Cyan;

                LeftTotem.Untuned = Untuned;
                RightTotem.Untuned = Untuned;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Ghost)
            {
                Alpha = ryukoy.Untuned ? (float) getGhostAlpha() : 0;
                LeftTotem.Alpha = Alpha;
                RightTotem.Alpha = Alpha;
                foreach (DrawableProjectile projectile in projectiles)
                    projectile.Alpha = ryukoy.Untuned ? projectile.Alpha > Alpha ? Alpha : 0 : 0;
            }
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            foreach (Drawable draw in CurrentPlayfield)
                if (draw is Buff buff)
                {
                    Vector2 buffPos = buff.ToSpaceOfOtherDrawable(Vector2.Zero, this);
                    double distance = Math.Sqrt(Math.Pow(buffPos.X, 2) + Math.Pow(buffPos.Y, 2));

                    if (distance < 0)
                        distance *= -1;

                    if (distance <= 32)
                    {
                        buff.Expire();
                        Heal(10);
                        Charge(10);
                    }
                }
        }

        //TODO: Share base logic?
        protected override DrawableBullet BulletAddRad(float speed, float angle, Color4 color, float size, float damage)
        {
            DrawableBullet drawableBullet = base.BulletAddRad(speed, angle, color, size, damage);

            drawableBullet.HitObject.Damage = Ghost ? 0 : damage;
            drawableBullet.HitObject.ColorOverride = Ghost ? Color4.Cyan : color;

            projectiles.Add(drawableBullet);
            drawableBullet.OnDispose += () => projectiles.Remove(drawableBullet);
            return drawableBullet;
        }

        private double getGhostAlpha()
        {
            const double alpha_max = 0.5d;
            const double alpha_min = 0d;

            const double range = 10;
            const double min_dist = 100;

            const double scale = (alpha_max - alpha_min) / (min_dist - range);

            return Math.Min(alpha_min + (souls - range) * scale, alpha_max);
        }
    }
}