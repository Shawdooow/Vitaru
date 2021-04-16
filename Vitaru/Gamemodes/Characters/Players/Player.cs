// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Settings;

namespace Vitaru.Gamemodes.Characters.Players
{
    public abstract class Player : Character
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

        public bool SpellActive { get; protected set; }

        public double MovementSpeedMultiplier = 1;

        public virtual string Ability => "None";

        public virtual string[] AbilityStats => null;

        public virtual Role Role { get; } = Role.Offense;

        public virtual Difficulty Difficulty { get; } = Difficulty.Easy;

        public virtual bool Implemented { get; } = false;

        public virtual string Background => "Default Background Text   C:<";

        public PlayerBinds Binds { get; set; }

        //Is reset after healing applied
        public float HealingMultiplier = 1;

        protected List<HealingProjectile> HealingProjectiles { get; private set; } = new();

        protected const float HEALING_FALL_OFF = 0.85f;

        private const float healing_range = 64f;
        private const float healing_min = 0.5f;
        private const float healing_max = 2f;

        private double lastQuarterBeat = -1;
        private double nextHalfBeat = -1;
        private double nextQuarterBeat = -1;

        private double shootTime;

        private readonly Vector2 border = GamemodeStore.SelectedGamemode?.Gamemode.GetGamefieldSize() / 2 ?? Vector2.One;

        private bool GOD_KING;

        protected DrawablePlayer DrawablePlayer { get; set; }

        public override void SetDrawable(DrawableGameEntity drawable)
        {
            DrawablePlayer = drawable as DrawablePlayer;
            base.SetDrawable(drawable);
        }

        public override DrawableGameEntity GenerateDrawable()
        {
            return new DrawablePlayer(this)
            {
                Position = Position
            };
        }

        protected Player(Gamefield gamefield) : base(gamefield)
        {
            GOD_KING = global::Vitaru.Vitaru.VitaruSettings.GetBool(VitaruSetting.DebugHacks);

            Position = new Vector2(0, 200);
            Team = PLAYER_TEAM;

            if (gamefield != null) Binds = new PlayerBinds();
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Energy = EnergyCapacity / 2f;
        }


        #region Beat

        public override void OnNewBeat()
        {
            base.OnNewBeat();

            OnHalfBeat();
            lastQuarterBeat = Clock.LastCurrent;
            nextHalfBeat = Clock.LastCurrent + TrackManager.CurrentTrack.Metadata.GetBeatLength() / 2;
            nextQuarterBeat = Clock.LastCurrent + TrackManager.CurrentTrack.Metadata.GetBeatLength() / 4;
        }

        protected virtual void OnHalfBeat()
        {
            nextHalfBeat = -1;
        }

        protected virtual void OnQuarterBeat()
        {
            lastQuarterBeat = nextQuarterBeat;
            nextQuarterBeat += TrackManager.CurrentTrack.Metadata.GetBeatLength() / 4;

            if (HealingProjectiles.Count > 0)
            {
                float fallOff = 1;

                for (int i = 0; i < HealingProjectiles.Count - 1; i++)
                    fallOff *= HEALING_FALL_OFF;

                foreach (HealingProjectile healingBullet in HealingProjectiles)
                {
                    Heal(GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff * HealingMultiplier);
                }

                HealingProjectiles = new List<HealingProjectile>();
                HealingMultiplier = 1;
            }
        }

        #endregion


        public override void Update()
        {
            base.Update();

            foreach (VitaruActions v in (VitaruActions[]) Enum.GetValues(typeof(VitaruActions)))
            {
                if (Binds[v] && !Binds.Last(v))
                    Pressed(v);
                else if (!Binds[v] && Binds.Last(v))
                    Released(v);
            }

            if (GOD_KING)
                Charge(999);

            if (nextHalfBeat <= Clock.LastCurrent && nextHalfBeat != -1)
                OnHalfBeat();

            if (nextQuarterBeat <= Clock.LastCurrent && nextQuarterBeat != -1)
                OnQuarterBeat();

            if (Binds[VitaruActions.Shoot] && Clock.LastCurrent >= shootTime)
                PatternWave();

            if (HealingProjectiles.Count > 0)
            {
                float fallOff = 1;

                for (int i = 0; i < HealingProjectiles.Count - 1; i++)
                    fallOff *= HEALING_FALL_OFF;

                foreach (HealingProjectile healingBullet in HealingProjectiles)
                    Charge((float) Clock.LastElapsedTime / 500 *
                           (GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff));
            }

            DrawablePlayer?.Seal.Update();

            Position = GetPositionOffset(0.3f);

            AudioManager.Context.Listener.Position = new Vector3(Position.X, 0, Position.Y);

            SpellUpdate();
        }

        protected override void ParseProjectile(Projectile projectile)
        {
            base.ParseProjectile(projectile);

            const int maxHeal = 64;
            float edgeDistance = float.MaxValue;

            switch (projectile)
            {
                default:
                    return;
                case Bullet bullet:

                    float min = bullet.Diameter / 2 + HitboxDiameter / 2;

                    if (Position.Y - bullet.Position.Y < min + maxHeal)
                        if (Position.X - bullet.Position.X < min + maxHeal)
                        {
                            float distance = Vector2.Distance(projectile.Position, Position);
                            edgeDistance = distance - min;
                        }

                    break;
            }

            if (edgeDistance < maxHeal)
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

            if (edgeDistance < projectile.MinDistance)
                projectile.MinDistance = edgeDistance;
        }

        protected virtual void PatternWave()
        {
            double half = TrackManager.CurrentTrack.Metadata.GetBeatLength() / 2;
            shootTime = Clock.LastCurrent + half;

            DrawablePlayer?.Seal.Shoot(half);

            const int numberbullets = 3;
            float directionModifier = -0.2f;

            float cursorAngle = 0;

            if (Binds[VitaruActions.Sneak])
            {
                cursorAngle = ((float) Math.Atan2(InputManager.Mouse.Position.Y - Position.Y,
                        InputManager.Mouse.Position.X - Position.X))
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
                BulletAddRad(1, (cursorAngle - 90).ToRadians() + directionModifier, color, size, damage, 800);

                if (Binds[VitaruActions.Sneak])
                    directionModifier += 0.1f;
                else
                    directionModifier += 0.2f;
            }
        }

        protected override void TakeDamage(float amount) => base.TakeDamage(GOD_KING ? 0 : amount);

        protected virtual void Charge(float amount) => Energy = Math.Clamp(Energy + amount, 0, EnergyCapacity);

        protected virtual void DrainEnergy(float amount) => Energy = Math.Clamp(Energy - amount, 0, EnergyCapacity);

        #region Input

        public void Pressed(VitaruActions t)
        {
            if (CheckSpellActivate(t))
                SpellActivate(t);

            DrawablePlayer?.Seal.Pressed(t);

            switch (t)
            {
                case VitaruActions.Sneak:
                    Drawable.HitboxOutline.FadeTo(1f, 200);
                    Drawable.Hitbox.FadeTo(1f, 200);
                    break;
                case VitaruActions.Shoot:
                    shootTime = Clock.LastCurrent;
                    break;
            }
        }

        public void Released(VitaruActions t)
        {
            if (CheckSpellDeactivate(t))
                SpellDeactivate(t);

            DrawablePlayer?.Seal.Released(t);

            switch (t)
            {
                case VitaruActions.Sneak:
                    Drawable.HitboxOutline.FadeTo(0f, 200);
                    Drawable.Hitbox.FadeTo(0f, 200);
                    break;
            }
        }

        protected virtual Vector2 GetPositionOffset(double playerSpeed)
        {
            Vector2 playerPosition = Position;

            double yTranslationDistance = playerSpeed * Clock.LastElapsedTime * MovementSpeedMultiplier;
            double xTranslationDistance = playerSpeed * Clock.LastElapsedTime * MovementSpeedMultiplier;

            if (Binds[VitaruActions.Sneak])
            {
                xTranslationDistance /= 2d;
                yTranslationDistance /= 2d;
            }

            if (Binds[VitaruActions.Up])
                playerPosition.Y -= (float) yTranslationDistance;
            if (Binds[VitaruActions.Down])
                playerPosition.Y += (float) yTranslationDistance;

            if (Binds[VitaruActions.Left])
                playerPosition.X -= (float) xTranslationDistance;
            if (Binds[VitaruActions.Right])
                playerPosition.X += (float) xTranslationDistance;

            playerPosition = Vector2.Clamp(playerPosition, -border, border);

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


        protected override void Dispose(bool finalize)
        {
            Binds?.Dispose();
            base.Dispose(finalize);
        }

        protected virtual float GetBulletHealingMultiplier(float value) =>
            PrionMath.Remap(value, 0, healing_range, healing_min, healing_max);

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