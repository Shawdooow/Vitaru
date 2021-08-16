// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Input;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Input;
using Vitaru.Play.Projectiles;
using Vitaru.Settings;

namespace Vitaru.Play.Characters.Players
{
    public abstract class Player : Character
    {
        public override string Name { get; set; } = nameof(Player);

        public const int PLAYER_TEAM = 1;

        public override Color PrimaryColor => Color.Navy;

        public override Color SecondaryColor => "#92a0dd".HexToColor();

        public override Color ComplementaryColor => "#d6d6d6".HexToColor();

        public double LastDamageTime { get; protected set; }

        public virtual float EnergyCapacity => 20f;

        public virtual float Energy { get; private set; }

        public virtual float EnergyCost { get; } = 4;

        public virtual float EnergyDrainRate { get; } = 0;

        public bool SpellActive { get; protected set; }

        public double MovementSpeedMultiplier = 1;

        public virtual string Ability => "None";

        public virtual string[] AbilityStats => null;

        public virtual Role Role => Role.Offense;

        public virtual Difficulty Difficulty => Difficulty.Easy;

        public virtual bool Implemented => false;

        public virtual string Background => "Default Background Text   C:<";

        public bool GetBind(VitaruActions action) => AI ? AIBinds[action] : PlayerBinds[action];

        public bool GetLastBind(VitaruActions action) => AI ? AILastBinds[action] : PlayerBinds.Last(action);

        protected PlayerBinds PlayerBinds { get; set; }

        protected Dictionary<VitaruActions, bool> AIBinds;

        protected Dictionary<VitaruActions, bool> AILastBinds;

        public virtual bool AI => false;

        private const int gridDivisorWidth = 160;
        private const int gridDivisorHeight = 90;

        private const float gridPositioningMargin = 2;

        protected Sprite Safe;

        //Is reset after healing applied
        public float HealingMultiplier = 1;

        protected List<HealingProjectile> HealingProjectiles { get; private set; } = new();

        protected const float HEALING_FALL_OFF = 0.5f;

        private const float healing_range = 64f;
        private const float healing_min = 0.5f;
        private const float healing_max = 4f;

        private double beat = 1000 / 60d;
        private double lastQuarterBeat = double.MinValue;
        private double nextHalfBeat = double.MinValue;
        private double nextQuarterBeat = double.MinValue;

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
            GOD_KING = Vitaru.VitaruSettings.GetBool(VitaruSetting.DebugHacks);

            Position = new Vector2(0, 200);
            Team = PLAYER_TEAM;
            CircularHitbox.Diameter = 6;

            if (AI)
            {
                AIBinds = new Dictionary<VitaruActions, bool>();
                AILastBinds = new Dictionary<VitaruActions, bool>();
                foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
                {
                    AIBinds[v] = false;
                    AILastBinds[v] = false;
                }
            }
            else if (gamefield != null) PlayerBinds = new PlayerBinds();
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Energy = EnergyCapacity / 2f;

            if (AI)
                Gamefield.OverlaysLayer.Add(Safe = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
                {
                    Size = new Vector2(25),
                    Color = ComplementaryColor
                });
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
            beat = TrackManager.CurrentTrack.Metadata.GetBeatLength();
            lastQuarterBeat = nextQuarterBeat;
            nextQuarterBeat += beat / 4;

            if (HealingProjectiles.Count > 0)
            {
                HealingProjectiles = new List<HealingProjectile>();
                HealingMultiplier = 1;
            }
        }

        #endregion


        public override void Update()
        {
            base.Update();

            if (AI) Bot();

            foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
            {
                if (GetBind(v) && !GetLastBind(v))
                    Pressed(v);
                else if (!GetBind(v) && GetLastBind(v))
                    Released(v);
            }

            if (GOD_KING)
                Charge(999);

            if (nextHalfBeat <= Clock.LastCurrent && nextHalfBeat != -1)
                OnHalfBeat();

            if (nextQuarterBeat <= Clock.LastCurrent && nextQuarterBeat != -1)
                OnQuarterBeat();

            if (GetBind(VitaruActions.Shoot) && Clock.LastCurrent >= shootTime)
                PatternWave();

            if (HealingProjectiles.Count > 0 && Gamefield.Current > LastDamageTime + beat * 2)
            {
                float fallOff = 1;

                if (Gamefield.Current > LastDamageTime + beat * 4)
                    foreach (HealingProjectile healingBullet in HealingProjectiles)
                    {
                        Heal((float)Clock.LastElapsedTime / 1000 * GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff * HealingMultiplier);
                        fallOff *= HEALING_FALL_OFF;
                    }

                fallOff = 1;

                foreach (HealingProjectile healingBullet in HealingProjectiles)
                {
                    Charge((float)Clock.LastElapsedTime / 1000 * (GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff));
                    fallOff *= HEALING_FALL_OFF;
                }
            }

            DrawablePlayer?.Seal.Update();

            Position = GetPositionOffset(0.3f);

            AudioManager.Context.Listener.Position = new Vector3(Position.X, 0, Position.Y);

            SpellUpdate();
        }


        #region Shooting


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

                    float min = bullet.CircularHitbox.Radius + Hitbox.Radius;

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

        protected virtual void PatternWave(int count = 3)
        {
            double half = TrackManager.CurrentTrack.Metadata.GetBeatLength() / 2;
            shootTime = Clock.LastCurrent + half;

            DrawablePlayer?.Seal.Shoot(half);

            float directionModifier = -0.2f * MathF.Round(count / 2f, MidpointRounding.ToZero);

            float cursorAngle = MathF.PI / -2;

            if (GetBind(VitaruActions.Sneak))
            {
                cursorAngle = (float) Math.Atan2(InputManager.Mouse.Position.Y - Position.Y,
                    InputManager.Mouse.Position.X - Position.X);
                directionModifier = -0.1f * MathF.Round(count / 2f, MidpointRounding.ToZero);
            }

            for (int i = 1; i <= count; i++)
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
                BulletAddRad(1, cursorAngle + directionModifier, color, size, damage, 800);

                if (GetBind(VitaruActions.Sneak))
                    directionModifier += 0.1f;
                else
                    directionModifier += 0.2f;
            }
        }


        #endregion


        protected override void TakeDamage(float amount) 
        {
            base.TakeDamage(GOD_KING ? 0 : amount);
            LastDamageTime = Gamefield.Current;
        }

        protected override void Die()
        {
            base.Die();
            DrawablePlayer.Hitbox.Color = Color.Red;
        }

        protected override void Rezzurect()
        {
            base.Rezzurect();
            DrawablePlayer.Hitbox.Color = Color.White;
        }

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

            if (GetBind(VitaruActions.Sneak))
            {
                xTranslationDistance /= 2d;
                yTranslationDistance /= 2d;
            }

            if (GetBind(VitaruActions.Up))
                playerPosition.Y -= (float) yTranslationDistance;
            if (GetBind(VitaruActions.Down))
                playerPosition.Y += (float) yTranslationDistance;

            if (GetBind(VitaruActions.Left))
                playerPosition.X -= (float) xTranslationDistance;
            if (GetBind(VitaruActions.Right))
                playerPosition.X += (float) xTranslationDistance;

            playerPosition = Vector2.Clamp(playerPosition, -border, border);

            return playerPosition;
        }


        #endregion


        protected virtual void Bot()
        {
            foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
                AILastBinds[v] = AIBinds[v];

            //Reset movement binds before we pick a new direction
            AIBinds[VitaruActions.Sneak] = false;
            AIBinds[VitaruActions.Up] = false;
            AIBinds[VitaruActions.Down] = false;
            AIBinds[VitaruActions.Left] = false;
            AIBinds[VitaruActions.Right] = false;

            if (Vitaru.FEATURES >= Features.Experimental)
                gridBot();
            else
                circleViewBot();
        }


        #region Circle View Bot


        //dist
        private const int minimums = 32;
        //ms
        private const double foresight = 10;

        /// <summary>
        /// Look around us and determine the direction with the least amount of bullets in it
        /// </summary>
        private void circleViewBot()
        {
            List<KeyValuePair<Projectile, HitResults>> n = new();

            foreach (Gamefield.ProjectilePack pack in Gamefield.ProjectilePacks)
            {
                if (pack.Team == Team) continue;

                IReadOnlyList<Projectile> projectiles = pack.Children;
                for (int i = 0; i < projectiles.Count; i++)
                {
                    Projectile projectile = projectiles[i];

                    if (!projectile.Active)
                        continue;

                    HitResults results;

                    switch (projectile)
                    {
                        default:
                            continue;
                        case Bullet b:
                            CircularHitbox hitbox = b.CircularHitbox;

                            //We don't actually give a shit where the bullets are now, we want to know where they will be
                            Vector2 f = b.GetPosition(Gamefield.Current + foresight);
                            hitbox.Position = f;

                            results = Hitbox.HitDetectionResults(hitbox);
                            results.Position = f;

                            //if they will be "close" then lets take them into account
                            if (results.EdgeDistance <= minimums)
                                n.Add(new KeyValuePair<Projectile, HitResults>(projectile, results));
                            break;
                    }
                }
            }

            //If nothing is nearby or things are very close SNEAK!
            if (!n.Any()) AIBinds[VitaruActions.Sneak] = true;

            foreach (KeyValuePair<Projectile, HitResults> p in n)
                if (p.Value.EdgeDistance <= minimums / 2f)
                    AIBinds[VitaruActions.Sneak] = true;

            Mounts direction = safestDirection(n);

            if (direction == Mounts.Center)
                direction = idleDirection();

            switch (direction)
            {
                case Mounts.TopLeft:
                    AIBinds[VitaruActions.Up] = true;
                    AIBinds[VitaruActions.Left] = true;
                    Safe.Position = new Vector2(-minimums) + Position;
                    break;
                case Mounts.TopCenter:
                    AIBinds[VitaruActions.Up] = true;
                    Safe.Position = new Vector2(0, -minimums) + Position;
                    break;
                case Mounts.TopRight:
                    AIBinds[VitaruActions.Up] = true;
                    AIBinds[VitaruActions.Right] = true;
                    Safe.Position = new Vector2(minimums, -minimums) + Position;
                    break;
                case Mounts.CenterLeft:
                    AIBinds[VitaruActions.Left] = true;
                    Safe.Position = new Vector2(-minimums, 0) + Position;
                    break;
                case Mounts.Center:
                    //do nothing
                    Safe.Position = Position;
                    break;
                case Mounts.CenterRight:
                    AIBinds[VitaruActions.Right] = true;
                    Safe.Position = new Vector2(minimums, 0) + Position;
                    break;
                case Mounts.BottomLeft:
                    AIBinds[VitaruActions.Down] = true;
                    AIBinds[VitaruActions.Left] = true;
                    Safe.Position = new Vector2(-minimums, minimums) + Position;
                    break;
                case Mounts.BottomCenter:
                    AIBinds[VitaruActions.Down] = true;
                    Safe.Position = new Vector2(0, minimums) + Position;
                    break;
                case Mounts.BottomRight:
                    AIBinds[VitaruActions.Down] = true;
                    AIBinds[VitaruActions.Right] = true;
                    Safe.Position = new Vector2(minimums, minimums) + Position;
                    break;
            }
        }

        //Degrees so I don't kill myself
        private const float fov = 45;
        private const float steps = 45;

        private Mounts safestDirection(List<KeyValuePair<Projectile, HitResults>> nearby)
        {
            //If nothing will be nearby then we are safe, don't move!
            if (!nearby.Any()) return Mounts.Center;

            //Next lets find somewhere we can go safely
            float[] angles = new float[nearby.Count];
            for (int i = 0; i < nearby.Count; i++)
            {
                Vector2 pos = nearby[i].Value.Position;
                float radian = MathF.Atan2(pos.Y - Position.Y, pos.X - Position.X) + Drawable.Rotation;
                float degree = radian.ToDegrees() + 90;

                degree = degree % 360;
                if (degree < 0) degree += 360;

                angles[i] = degree;
            }

            int[] density = new int[(int)((360 - steps) / steps)];

            //Calculate approximate density around us
            for (float i = 0; i < 360 - steps; i += steps)
            {
                for (int j = 0; j < angles.Length; j++)
                {
                    float low = i - fov / 2;
                    float high = i + fov / 2;

                    if (low < 0) low += 360;

                    if (angles[j] >= low && angles[j] <= high)
                        density[(int)(i / steps)]++;
                }
            }

            KeyValuePair<int, int> lowest = new(int.MaxValue, int.MaxValue);

            //Finally pick the least dense direction
            for (int i = 0; i < density.Length; i++)
            {
                if (density[i] <= lowest.Value)
                    lowest = new KeyValuePair<int, int>(i, density[i]);
            }

            switch (lowest.Key)
            {
                default:
                    return Mounts.Center;
                case 0:
                    return Mounts.TopCenter;
                case 1:
                    return Mounts.TopRight;
                case 2:
                    return Mounts.CenterRight;
                case 3:
                    return Mounts.BottomRight;
                case 4:
                    return Mounts.BottomCenter;
                case 5:
                    return Mounts.BottomLeft;
                case 6:
                    return Mounts.CenterLeft;
                case 7:
                    return Mounts.TopLeft;
            }
        }

        private const float margin = 100;

        private Mounts idleDirection()
        {
            Vector2 size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();

            int y = h();
            switch (w())
            {
                default:
                    return Mounts.Center;

                case -1 when y == -1:
                    return Mounts.TopLeft;
                case -1 when y == 0:
                    return Mounts.CenterLeft;
                case -1 when y == 1:
                    return Mounts.BottomLeft;

                case 0 when y == -1:
                    return Mounts.TopCenter;
                case 0 when y == 0:
                    return Mounts.Center;
                case 0 when y == 1:
                    return Mounts.BottomCenter;

                case 1 when y == -1:
                    return Mounts.TopRight;
                case 1 when y == 0:
                    return Mounts.CenterRight;
                case 1 when y == 1:
                    return Mounts.BottomRight;
            }

            int w()
            {
                if (Position.X < size.X / -2 + margin)
                    return 1; //right
                if (Position.X > size.X / 2 - margin)
                    return -1; //left
                return 0;
            }

            int h()
            {
                if (Position.Y < 0 + margin)
                    return 1; //down
                if (Position.Y > size.Y / 2 - margin)
                    return -1; //up
                return 0;
            }
        }



        #endregion


        #region Grid Bot


        /// <summary>
        /// Use a grid system to determine a safe spot for us to travel to using pathing AI
        /// </summary>
        private void gridBot()
        {
            Vector2 playfield = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();

            float gridWidth = playfield.X / gridDivisorWidth;
            float gridHeight = playfield.Y / gridDivisorHeight;
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
            PlayerBinds?.Dispose();
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
        Extreme,
        Impossible,
    }
}