// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System;
using System.Collections.Generic;
using Vitaru.Gamemodes.Dodge;

#endregion

namespace Vitaru.Gamemodes.Vitaru.HitObjects.DrawableHitObjects
{
    public class DrawableVitaruCluster : DrawableCluster
    {
        private StarPiece starPiece;

        private bool done;

        private int currentRepeat;

        private Enemy enemy;

        public readonly VitaruCluster HitObject;

        //cluster + this + enemy
        protected override double object_size => 216d + 1166.6d + 1188.32d;

        //TODO: using this one causes everything to be fucked
        public DrawableVitaruCluster(VitaruCluster cluster) : base(cluster)
        {
        }

        public DrawableVitaruCluster(VitaruCluster cluster, VitaruPlayfield playfield) : base(cluster, playfield)
        {
            HitObject = cluster;

            Size = new Vector2(30);
            Position = HitObject.Position;
            Alpha = 0;

            HitObject.PositionChanged += _ =>
            {
                Position = HitObject.Position;
                //TODO: Move bullets too?
            };

            if (!cluster.IsSlider && !cluster.IsSpinner)
            {
                double endTime = HitObject.StartTime + HitObject.TimePreempt * 2 - HitObject.TimeFadein;
                HitObject.EndTime = endTime;
            }

            if (cluster.IsSlider)
            {
                foreach (SampleInfo info in cluster.GetRepeatSamples(0))
                {
                    SymcolSkinnableSound sound;
                    SymcolSkinnableSounds.Add(sound = GetSkinnableSound(info));
                    AddInternal(sound);
                }
            }
            else
            {
                foreach (SampleInfo info in cluster.BetterSamples)
                {
                    SymcolSkinnableSound sound;
                    SymcolSkinnableSounds.Add(sound = GetSkinnableSound(info));
                    AddInternal(sound);
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (HitObject.IsSlider)
            {
                double completionProgress =
                    MathHelper.Clamp((VitaruPlayfield.Current - HitObject.StartTime) / HitObject.Duration, 0, 1);
                int repeat = HitObject.SpanAt(completionProgress);

                if (Started && !done)
                {
                    starPiece.Position = HitObject.PositionAt(completionProgress);
                    if (!(ChapterSet is DodgeChapterSet))
                        enemy.Position = HitObject.PositionAt(completionProgress);
                }

                if (repeat > currentRepeat)
                {
                    PlayBetterRepeatSamples(repeat + 1);
                    currentRepeat = repeat;
                }

                if (HitObject.EndTime <= VitaruPlayfield.Current && Started && !done)
                    done = true;
            }
        }

        protected void PlayBetterRepeatSamples(int repeat)
        {
            PlayBetterSamples();

            foreach (SymcolSkinnableSound sound in SymcolSkinnableSounds)
                sound.Delete();

            SymcolSkinnableSounds = new List<SymcolSkinnableSound>();

            foreach (SampleInfo info in HitObject.GetRepeatSamples(repeat))
            {
                SymcolSkinnableSound sound;

                HitObject.SampleControlPoint = HitObject.GetSampleControlPoint(repeat);

                SymcolSkinnableSounds.Add(sound = GetSkinnableSound(info));
                AddInternal(sound);
            }
        }

        protected override void Preempt()
        {
            base.Preempt();

            if (!disable_bullets)
                foreach (HitObject o in HitObject.NestedHitObjects)
                {
                    if (o is Bullet b)
                    {
                        if (VitaruPlayfield.BOUNDLESS || ChapterSet is DodgeChapterSet)
                        {
                            b.Angle += getPlayerAngle() - (float) Math.PI / 2;
                            b.SliderType = b.SliderType;
                        }

                        if (b.ShootPlayer)
                            b.Position = VitaruPlayfield.PlayerPosition;

                        DrawableBullet drawableBullet = ChapterSet.GetDrawableBullet(b, VitaruPlayfield);
                        CurrentPlayfield.Add(drawableBullet);
                    }
                    else if (o is Laser l)
                    {
                        if (VitaruPlayfield.BOUNDLESS || ChapterSet is DodgeChapterSet)
                            l.Angle += getPlayerAngle() - (float) Math.PI / 2;

                        DrawableLaser drawableLaser = ChapterSet.GetDrawableLaser(l, VitaruPlayfield);
                        CurrentPlayfield.Add(drawableLaser);
                    }
                }

            CurrentPlayfield.Add(starPiece = new StarPiece
            {
                Alpha = 0,
                Masking = true,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.Centre,
                Size = Size,
                Colour = AccentColour
            });

            if (!(ChapterSet is DodgeChapterSet))
            {
                CurrentPlayfield.Add(enemy = new Enemy(VitaruPlayfield, this)
                {
                    Alpha = 0,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.Centre,
                    Depth = 5,
                    //MaxHealth = cluster.EnemyHealth,
                    Team = HitObject.Team
                });

                enemy.Position = getClusterStartPosition();
                enemy.FadeIn(Math.Min(HitObject.TimeFadein * 2, HitObject.TimePreempt))
                    .MoveTo(HitObject.Position, HitObject.TimePreempt, Easing.OutSine);
            }
            else
            {
                starPiece.FadeInFromZero(Math.Min(HitObject.TimeFadein * 2, HitObject.TimePreempt))
                    .MoveTo(HitObject.Position, HitObject.TimePreempt);
            }

            this.FadeInFromZero(Math.Min(HitObject.TimeFadein * 2, HitObject.TimePreempt));

            starPiece.Position = getClusterStartPosition();

            Position = HitObject.Position;
            Size = new Vector2(64);
        }

        protected override void Start()
        {
            base.Start();

            if (!HitObject.IsSlider && !HitObject.IsSpinner)
                PlayBetterSamples();
            else
                PlayBetterRepeatSamples(1);
        }

        protected override void End()
        {
            base.End();

            if (HitObject.IsSpinner)
                PlayBetterSamples();

            if (!(ChapterSet is DodgeChapterSet))
                enemy.MoveTo(getClusterStartPosition(), HitObject.TimeUnPreempt * 2, Easing.InQuad)
                    .ScaleTo(new Vector2(0.5f), HitObject.TimeUnPreempt, Easing.InQuad)
                    .FadeOut(HitObject.TimeUnPreempt, Easing.InQuad);

            this.FadeOut(HitObject.TimeUnPreempt / 2)
                .MoveTo(getClusterStartPosition(), HitObject.TimeUnPreempt * 2, Easing.InQuad);

            starPiece.FadeOut(HitObject.TimeUnPreempt / 2)
                .ScaleTo(new Vector2(0.1f), HitObject.TimeUnPreempt / 2);
        }

        protected override void UnPreempt()
        {
            base.UnPreempt();
            Die();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;

            if (VitaruPlayfield != null)
            {
                if (enemy != null)
                {
                    enemy.Clear();
                    enemy.Dispose();
                    enemy = null;
                }

                if (!starPiece.IsDisposed)
                {
                    starPiece.Clear();
                    CurrentPlayfield.Remove(starPiece);
                    starPiece.Dispose();
                    starPiece = null;
                }

                VitaruPlayfield.Remove(this);
            }

            base.Dispose(isDisposing);
        }

        public void Death(Enemy enemy)
        {
            ApplyResult(r =>
            {
                VitaruJudgementResult v = (VitaruJudgementResult) r;
                v.Type = HitResult.Great;
                v.VitaruJudgement.BonusScore = true;
            });
            if (VitaruPlayfield.Current < HitObject.StartTime && !Started)
            {
                enemy.FadeOut(HitObject.StartTime - VitaruPlayfield.Current);
                starPiece.FadeInFromZero(HitObject.StartTime - VitaruPlayfield.Current);
            }
            else if (VitaruPlayfield.Current < HitObject.EndTime && Started)
            {
                enemy.FadeOutFromOne(200)
                    .ScaleTo(new Vector2(1.5f), 200);
                starPiece.FadeInFromZero(200);
            }
            else if (VitaruPlayfield.Current >= HitObject.EndTime)
            {
                enemy.ClearTransforms();
                enemy.FadeOutFromOne(200);
            }
        }

        private Vector2 getClusterStartPosition()
        {
            if (HitObject.Position.X <= 384f / 2 && HitObject.Position.Y <= 512f / 2)
                return HitObject.Position - new Vector2(384f / 2, 512f / 2);
            if (HitObject.Position.X > 384f / 2 && HitObject.Position.Y <= 512f / 2)
                return new Vector2(HitObject.Position.X + 384f / 2, HitObject.Position.Y - 512f / 2);
            if (HitObject.Position.X > 384f / 2 && HitObject.Position.Y > 512f / 2)
                return HitObject.Position + new Vector2(384f / 2, 512f / 2);

            return new Vector2(HitObject.Position.X - 384f / 2, HitObject.Position.Y + 512f / 2);
        }

        private float getPlayerAngle()
        {
            if (VitaruPlayfield.Player == null)
                return 0;
            return (float) Math.Atan2(VitaruPlayfield.PlayerPosition.Y - Position.Y,
                VitaruPlayfield.PlayerPosition.X - Position.X);
        }
    }
}