// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game.Audio;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Input.Events;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Tracks;

namespace Vitaru.Gamemodes.Characters.Players
{
    public abstract class Player : Character, IHasInputKeys<VitaruActions>, IHasInputMousePosition
    {
        public override string Name { get; set; } = nameof(Player);

        public const int PLAYER_TEAM = 1;

        public override float HitboxDiameter => 6f;

        public override Color PrimaryColor => Color.Navy;

        public override Color SecondaryColor => "#92a0dd".HexToColor();

        public override Color ComplementaryColor => "#d6d6d6".HexToColor();

        public virtual float EnergyCapacity => 20f;

        public virtual float Energy { get; private set; }

        public virtual float EnergyCost { get; } = 4;

        public virtual float EnergyDrainRate { get; } = 0;

        protected bool SpellActive { get; set; }

        public double MovementSpeedMultiplier = 1;

        public virtual string Ability => "None";

        public virtual string AbilityStats => null;

        public virtual Role Role { get; } = Role.Offense;

        public virtual Difficulty Difficulty { get; } = Difficulty.Easy;

        public virtual string Background => "Default Background Text   C:<";

        public BindInputHandler<VitaruActions> InputHandler { get; set; }

        //Is reset after healing applied
        public double HealingMultiplier = 1;

        protected List<HealingProjectile> HealingProjectiles { get; private set; } = new List<HealingProjectile>();

        protected const float HEALING_FALL_OFF = 0.85f;

        private const float healing_range = 64;
        private const float healing_min = 0.5f;
        private const float healing_max = 2f;

        private Vector2 cursor = Vector2.Zero;

        private double shootTime;

        protected DrawablePlayer DrawablePlayer => (DrawablePlayer) Drawable;

        public virtual DrawablePlayer GenerateDrawable()
        {
            DrawablePlayer draw = new DrawablePlayer(this)
            {
                Position = new Vector2(0, 200)
            };
            Drawable = draw;
            return draw;
        }

        protected Player(Gamefield gamefield) : base(gamefield)
        {
            Team = PLAYER_TEAM;
            InputHandler = new VitaruInputManager();
            InputHandler.Add(this);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Energy = EnergyCapacity / 2f;
        }

        public override void Update()
        {
            base.Update();

            if (InputHandler.Actions[VitaruActions.Shoot] && Clock.Current >= shootTime)
                PatternWave();

            //TODO: fix this being needed?
            if (Drawable == null) return;

            if (HealingProjectiles.Count > 0)
            {
                float fallOff = 1;

                for (int i = 0; i < HealingProjectiles.Count - 1; i++)
                    fallOff *= HEALING_FALL_OFF;

                foreach (HealingProjectile healingBullet in HealingProjectiles)
                    Charge((float) Clock.LastElapsedTime / 500 *
                           (GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff));
            }

            DrawablePlayer.SignSprite.Rotation = (float) (-Clock.LastCurrent / 1000);

            Drawable.Position = GetNewPlayerPosition(0.3f);

            SpellUpdate();
        }

        protected override void ParseProjectile(Projectile projectile)
        {
            base.ParseProjectile(projectile);

            Vector2 difference = projectile.Position - Drawable.Position;

            float distance = (float) Math.Sqrt(Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2));
            float edgeDistance;

            switch (projectile)
            {
                default:
                    return;
                case Bullet bullet:
                    edgeDistance = distance - (bullet.Diameter / 2 + HitboxDiameter / 2);
                    break;
            }

            if (edgeDistance < 64)
            {
                bool add = true;
                foreach (HealingProjectile healingProjectile in HealingProjectiles)
                    if (healingProjectile.Projectile == projectile)
                    {
                        healingProjectile.EdgeDistance = edgeDistance;
                        add = false;
                    }

                if (add)
                    HealingProjectiles.Add(new HealingProjectile(projectile, edgeDistance));
            }

            //if (ChapterSet is DodgeChapterSet)
            //    edgeDistance *= 1.5f;

            if (edgeDistance < projectile.MinDistance)
                projectile.MinDistance = edgeDistance;
        }

        protected virtual void PatternWave()
        {
            shootTime = Clock.Current + Track.Level.GetBeatLength() / 4;

            const int numberbullets = 3;
            float directionModifier = -0.2f;

            float cursorAngle = 0;

            if (InputHandler.Actions[VitaruActions.Sneak])
            {
                cursorAngle = ((float) Math.Atan2(cursor.Y - Drawable.Position.Y, cursor.X - Drawable.Position.X))
                    .ToDegrees() + 90;
                directionModifier = -0.1f;
            }

            for (int i = 1; i <= numberbullets; i++)
            {
                float size;
                float damage;
                Color color;

                if (i % 2 == 0)
                {
                    size = 28;
                    damage = 24;
                    color = PrimaryColor;
                }
                else
                {
                    size = 20;
                    damage = 18;
                    color = SecondaryColor;
                }

                //-90 = up
                BulletAddRad(1, (cursorAngle - 90).ToRadians() + directionModifier, color, size, damage);

                if (InputHandler.Actions[VitaruActions.Sneak])
                    directionModifier += 0.1f;
                else
                    directionModifier += 0.2f;
            }
        }

        protected virtual void Charge(float amount)
        {
            Energy = Math.Clamp(Energy + amount, 0, EnergyCapacity);
            ((DrawablePlayer) Drawable).SignSprite.Alpha = PrionMath.Scale(Energy, 0, EnergyCapacity);
        }

        protected virtual void DrainEnergy(float amount)
        {
            Energy = Math.Clamp(Energy - amount, 0, EnergyCapacity);
            DrawablePlayer.SignSprite.Alpha = PrionMath.Scale(Energy, 0, EnergyCapacity);
        }


        #region Input

        public bool Pressed(VitaruActions t)
        {
            if (CheckSpellActivate(t))
                SpellActivate(t);

            switch (t)
            {
                default:
                    return true;
                case VitaruActions.Sneak:
                    Drawable.Hitbox.FadeTo(1f, 200);
                    return true;
                case VitaruActions.Shoot:
                    PatternWave();
                    return true;
            }
        }

        public bool Released(VitaruActions t)
        {
            if (CheckSpellDeactivate(t))
                SpellDeactivate(t);

            switch (t)
            {
                default:
                    return true;

                case VitaruActions.Sneak:
                    Drawable.Hitbox.ClearTransforms();
                    Drawable.Hitbox.FadeTo(0f, 200);
                    return true;
            }
        }

        public void OnMouseMove(MousePositionEvent e) => cursor = e.Position;

        protected virtual Vector2 GetNewPlayerPosition(double playerSpeed)
        {
            Vector2 playerPosition = Drawable.Position;

            double yTranslationDistance = playerSpeed * Clock.LastElapsedTime * MovementSpeedMultiplier;
            double xTranslationDistance = playerSpeed * Clock.LastElapsedTime * MovementSpeedMultiplier;

            if (InputHandler.Actions[VitaruActions.Sneak])
            {
                xTranslationDistance /= 2d;
                yTranslationDistance /= 2d;
            }

            if (InputHandler.Actions[VitaruActions.Up])
                playerPosition.Y -= (float) yTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Down])
                playerPosition.Y += (float) yTranslationDistance;

            if (InputHandler.Actions[VitaruActions.Left])
                playerPosition.X -= (float) xTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Right])
                playerPosition.X += (float) xTranslationDistance;

            //if (!VitaruPlayfield.BOUNDLESS)
            //{
            //    playerPosition = Vector2.ComponentMin(playerPosition, ChapterSet.PlayfieldBounds.Zw);
            //    playerPosition = Vector2.ComponentMax(playerPosition, ChapterSet.PlayfieldBounds.Xy);
            //}

            return playerPosition;
        }

        #endregion


        #region Spell Handling

        /// <summary>
        ///     Called to see if a spell should go active
        /// </summary>
        protected virtual bool CheckSpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.Spell && Energy >= EnergyCost)
                return true;
            return false;
        }

        /// <summary>
        ///     Called to see if a spell should be deactivated
        /// </summary>
        /// <param name="action"></param>
        protected virtual bool CheckSpellDeactivate(VitaruActions action)
        {
            if (action == VitaruActions.Spell)
                return true;
            return false;
        }

        /// <summary>
        ///     Called when a spell is activated
        /// </summary>
        /// <param name="action"></param>
        protected virtual void SpellActivate(VitaruActions action)
        {
            SpellActive = true;
            if (EnergyDrainRate == 0)
                DrainEnergy(EnergyCost);
        }

        protected virtual void SpellUpdate()
        {
            if (Energy <= 0)
            {
                Energy = 0;
                SpellDeactivate(VitaruActions.Spell);
            }
        }

        /// <summary>
        ///     Called when a spell is deactivated
        /// </summary>
        /// <param name="action"></param>
        protected virtual void SpellDeactivate(VitaruActions action)
        {
            SpellActive = false;
        }

        #endregion


        protected virtual float GetBulletHealingMultiplier(float value) =>
            PrionMath.Scale(value, 0, healing_range, healing_min, healing_max);

        protected class HealingProjectile
        {
            public readonly Projectile Projectile;

            public float EdgeDistance { get; set; }

            public HealingProjectile(Projectile projectile, float distance)
            {
                Projectile = projectile;
                EdgeDistance = distance;
            }
        }
    }

    public enum Role
    {
        Offense,
        Defense,
        Support,
        Specialized
    }

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Insane,
        Another,
        Extra,

        //Crazy Town
        [Description("Time Freeze")] TimeFreeze,
        [Description("Arcanum Barrier")] ArcanumBarrier,

        //No
        [Description("Centipede")] Centipede,
        [Description("Serious")] SeriousShit
    }
}