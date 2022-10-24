// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Utilities;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Input;
using Vitaru.Play.Projectiles;
using Vitaru.Settings;

namespace Vitaru.Play.Characters.Players
{
    public abstract class Player : Character
    {
        #region Fields


        public override string Name { get; set; } = nameof(Player);

        public const int PLAYER_TEAM = 1;

        public override Color PrimaryColor => Color.Navy;

        public override Color SecondaryColor => "#92a0dd".HexToColor();

        public override Color ComplementaryColor => "#d6d6d6".HexToColor();

        public virtual string Seal => "Gameplay\\seal default.png";

        public virtual float SealRotationSpeed => 1f;

        public virtual string HealthRing => "Gameplay\\inner.png";

        public virtual string EnergyRing => "Gameplay\\outer.png";

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

        public virtual string Notes => string.Empty;

        public virtual ImplementationState ImplementationState => ImplementationState.NotFunctioning;

        public virtual string OriginMedia => string.Empty;

        public virtual string Description => string.Empty;

        public bool GetBind(VitaruActions action) => AI ? AIBinds[action] : Vitaru.PlayerBinds[action];

        public bool GetLastBind(VitaruActions action) => AI ? AILastBinds[action] : Vitaru.PlayerBinds.Last(action);

        protected Dictionary<VitaruActions, bool> AIBinds;

        protected Dictionary<VitaruActions, bool> AILastBinds;

        public virtual bool AI { get; set; }

        private const int gridDivisorWidth = 1024 / 8;
        private const int gridDivisorHeight = 820 / 8;

        private const float tilePositioningMargin = 2;

        public List<Vector2> TargetPositions { get; set; } = new();
        protected int Target { get; set; }

        //protected Box Grid;
        //protected List<Sprite> Path = new List<Sprite>();
        //protected Sprite Target;
        //protected Sprite Safe;

        //Is reset after healing applied
        public float HealingMultiplier = 1;

        protected List<HealingProjectile> HealingProjectiles { get; private set; } = new();

        protected const float HEALING_FALL_OFF = 0.5f;

        private const float healing_range = 64f;
        private const float healing_min = 0.5f;
        private const float healing_max = 4f;

        private double beat = 1000 / 60d;

        private double shootTime;

        private readonly Vector2
            border = GamemodeStore.SelectedGamemode?.Gamemode.GetGamefieldSize() / 2 ?? Vector2.One;

        private bool GOD;
        private bool KING;

        public DrawablePlayer DrawablePlayer { get; protected set; }


        #endregion


        public override void SetDrawable(DrawableGameEntity drawable)
        {
            DrawablePlayer = drawable as DrawablePlayer;
            base.SetDrawable(drawable);
        }

        protected Player(PlayManager manager) : base(manager)
        {
            GOD = Vitaru.VitaruSettings.GetBool(VitaruSetting.GodHacks);
            KING = Vitaru.VitaruSettings.GetBool(VitaruSetting.KingHacks);

            Position = new Vector2(0, 200);
            Team = PLAYER_TEAM;
            CircularHitbox.Diameter = 6;

            AIBinds = new Dictionary<VitaruActions, bool>();
            AILastBinds = new Dictionary<VitaruActions, bool>();

            foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
            {
                AIBinds[v] = false;
                AILastBinds[v] = false;
            }
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
        }


        #region Overrides


        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Energy = EnergyCapacity / 2f;

            //if (AI)
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Sprite node = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            //        {
            //            Size = new Vector2(20),
            //            Color = ComplementaryColor,
            //            Alpha = 0
            //        };
            //
            //        Path.Add(node);
            //        Gamefield.OverlaysLayer.Add(node);
            //    }
            //        
            //
            //    Gamefield.OverlaysLayer.Add(Grid = new Box()
            //    {
            //        Origin = Mounts.TopLeft,
            //        Alpha = 0.2f,
            //        Color = PrimaryColor
            //    });
            //
            //    Gamefield.OverlaysLayer.Add(Target = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            //    {
            //        Size = new Vector2(36),
            //        Color = PrimaryColor
            //    });
            //
            //    Gamefield.OverlaysLayer.Add(Safe = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            //    {
            //        Size = new Vector2(20),
            //        Color = ComplementaryColor
            //    });
            //}
        }

        public override void Update()
        {
            if (AI) Bot();

            foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
            {
                if (GetBind(v) && !GetLastBind(v))
                    Pressed(v);
                else if (!GetBind(v) && GetLastBind(v))
                    Released(v);
            }

            if (KING)
                Charge(999);

            if (GetBind(VitaruActions.Shoot) && Clock.LastCurrent >= shootTime)
                PatternWave();

            if (HealingProjectiles.Count > 0 && PlayManager.Current > LastDamageTime + beat * 2)
            {
                float fallOff = 1;

                if (PlayManager.Current > LastDamageTime + beat * 4)
                    foreach (HealingProjectile healingBullet in HealingProjectiles)
                    {
                        Heal((float)Clock.LastElapsedTime / 1000 *
                             GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff * HealingMultiplier);
                        fallOff *= HEALING_FALL_OFF;
                    }

                fallOff = 1;

                foreach (HealingProjectile healingBullet in HealingProjectiles)
                {
                    Charge((float)Clock.LastElapsedTime / 1000 *
                           (GetBulletHealingMultiplier(healingBullet.EdgeDistance) * fallOff));
                    fallOff *= HEALING_FALL_OFF;
                }
            }

            //DrawablePlayer?.UpdateAnimations(this);

            Position = GetPositionOffset(0.3f);

            SpellUpdate();
        }


        #endregion


        #region Beat


        public override void OnNewBeat()
        {
            base.OnNewBeat();
            beat = TrackManager.CurrentTrack.Metadata.GetBeatLength();
        }

        public virtual void OnHalfBeat() { }

        public virtual void OnQuarterBeat()
        {
            if (HealingProjectiles.Count > 0)
            {
                HealingProjectiles = new List<HealingProjectile>();
                HealingMultiplier = 1;
            }
        }


        #endregion


        #region Shooting


        protected virtual void PatternWave(int count = 3)
        {
            double half = TrackManager.CurrentTrack.Metadata.GetBeatLength() / 2;
            shootTime = Clock.LastCurrent + half;

            float directionModifier = -0.2f * MathF.Round(count / 2f, MidpointRounding.ToZero);

            float cursorAngle = MathF.PI / -2;

            if (GetBind(VitaruActions.Sneak))
            {
                cursorAngle = (float)Math.Atan2(InputManager.Mouse.Position.Y - Position.Y,
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
                //BulletAddRad(1, cursorAngle + directionModifier, color, size, damage, 800);

                if (GetBind(VitaruActions.Sneak))
                    directionModifier += 0.1f;
                else
                    directionModifier += 0.2f;
            }
        }


        #endregion


        #region Stats


        protected override void TakeDamage(float amount)
        {
            base.TakeDamage(GOD ? 0 : amount);
            LastDamageTime = PlayManager.Current;
        }

        protected override void Die()
        {
            base.Die();
            DrawablePlayer.HitboxColor = Color.Red;
        }

        protected override void Rezzurect()
        {
            base.Rezzurect();
            DrawablePlayer.HitboxColor = Color.White;
        }

        protected virtual void Charge(float amount) => Energy = Math.Clamp(Energy + amount, 0, EnergyCapacity);

        protected virtual void DrainEnergy(float amount) => Energy = Math.Clamp(Energy - amount, 0, EnergyCapacity);


        #endregion


        #region Input


        public void Pressed(VitaruActions t)
        {
            if (CheckSpellActivate(t))
                SpellActivate(t);

            //DrawablePlayer?.Pressed(t);

            switch (t)
            {
                case VitaruActions.Sneak:
                    new FloatTransform(a => Drawable.HitboxAlpha = a, Drawable.HitboxAlpha, 1, this, PlayManager.Current, 200, Easings.None);
                    break;
                case VitaruActions.Shoot:
                    Renderer.CurrentRoot.Cursor.Hover(PrimaryColor);
                    shootTime = Clock.LastCurrent;
                    break;
            }
        }

        public void Released(VitaruActions t)
        {
            if (CheckSpellDeactivate(t))
                SpellDeactivate(t);

            //DrawablePlayer?.Released(t);

            switch (t)
            {
                case VitaruActions.Sneak:
                    new FloatTransform(a => Drawable.HitboxAlpha = a, Drawable.HitboxAlpha, 0, this, PlayManager.Current, 200, Easings.None);
                    break;
                case VitaruActions.Shoot:
                    Renderer.CurrentRoot.Cursor.HoverLost();
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
                playerPosition.Y -= (float)yTranslationDistance;
            if (GetBind(VitaruActions.Down))
                playerPosition.Y += (float)yTranslationDistance;

            if (GetBind(VitaruActions.Left))
                playerPosition.X -= (float)xTranslationDistance;
            if (GetBind(VitaruActions.Right))
                playerPosition.X += (float)xTranslationDistance;

            playerPosition = Vector2.Clamp(playerPosition, -border, border);

            return playerPosition;
        }


        #endregion


        protected virtual void Bot()
        {
            foreach (VitaruActions v in (VitaruActions[])Enum.GetValues(typeof(VitaruActions)))
                AILastBinds[v] = AIBinds[v];

            //Reset movement binds before we pick a new direction
            AIBinds[VitaruActions.Shoot] = false;
            AIBinds[VitaruActions.Up] = false;
            AIBinds[VitaruActions.Down] = false;
            AIBinds[VitaruActions.Left] = false;
            AIBinds[VitaruActions.Right] = false;

            //gridBot();
        }


        #region Grid Bot


        /// <summary>
        ///     Use a grid system to determine a safe spot for us to travel to using pathing AI
        /// </summary>
        //private void gridBot()
        //{
        //    AIBinds[VitaruActions.Sneak] = false;
        //
        //    AIBinds[VitaruActions.Shoot] = true;
        //
        //    Vector2 target = TargetPositions[Target];
        //
        //    if (Vector2.Distance(Position, target) <= 8)
        //    {
        //        Target++;
        //        if (Target >= TargetPositions.Count) Target = 0;
        //    }
        //
        //    Vector2 playfield = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();
        //
        //    float tileWidth = playfield.X / gridDivisorWidth;
        //    float tileHeight = playfield.Y / gridDivisorHeight;
        //
        //    int[,] tiles = new int[gridDivisorWidth, gridDivisorHeight];
        //
        //    //The tile the player is in
        //    Vector2Int playerTile = new(-1);
        //
        //    //first we want to locate the tile the player is in so we can just check for projectiles near it
        //    for (int x = 0; x < gridDivisorWidth; x++)
        //    {
        //        int tileX = x - gridDivisorWidth / 2;
        //
        //        for (int y = 0; y < gridDivisorHeight; y++)
        //        {
        //            int tileY = y - gridDivisorHeight / 2;
        //
        //            //check if the player is here
        //            if (Position.X >= tileX * tileWidth && Position.X <= (tileX + 1) * tileWidth &&
        //                Position.Y >= tileY * tileHeight && Position.Y <= (tileY + 1) * tileHeight)
        //            {
        //                playerTile = new Vector2Int(x, y);
        //                break;
        //            }
        //        }
        //
        //        if (playerTile.X != -1) break;
        //    }
        //
        //    //how many tiles (x2) across is the grid
        //    const int view = 12;
        //    //how much extra space around projectiles should we treat as part of the projectile and avoid?
        //    const float pMargin = 4;
        //
        //    //check for projectiles that intersect total view area first and then sub-check those against the tiles
        //
        //    //Grid hitbox
        //    RectangularHitbox grid = new()
        //    {
        //        Size = new Vector2(tileWidth, tileHeight) * view * 2,
        //        Position = new Vector2((playerTile.X - view) * tileWidth - (playfield.X / 2),
        //            (playerTile.Y - view) * tileHeight - (playfield.Y / 2)),
        //    };
        //
        //    //Grid.Size = grid.Size;
        //    //Grid.Position = grid.Position;
        //
        //    List<RectangularHitbox> nearby = new();
        //    //look ahead 1/60 a second
        //    double futureTime = Gamefield.Current + 1000 / 60d;
        //
        //    //now check if any projectiles are intersecting this grid tile
        //    foreach (Gamefield.ProjectilePack pack in Gamefield.ProjectilePacks)
        //    {
        //        if (pack.Team == Team) continue;
        //
        //        foreach (Projectile projectile in pack.Children)
        //        {
        //            //if the projectile is here increase the tile's "density" so we know to avoid it
        //            switch (projectile)
        //            {
        //                case Bullet bullet:
        //
        //                    if (!bullet.Active) continue;
        //
        //                    float radius = bullet.CircularHitbox.Radius + pMargin / 2;
        //
        //                    RectangularHitbox box = new()
        //                    {
        //                        Size = new Vector2(bullet.CircularHitbox.Diameter + pMargin),
        //                        Position = new Vector2(bullet.CircularHitbox.Position.X - radius,
        //                            bullet.CircularHitbox.Position.Y - radius),
        //                    };
        //
        //                    if (grid.HitDetectionResults(box))
        //                        nearby.Add(box);
        //
        //                    //fast-forward and check again
        //                    Vector2 futurePos = bullet.GetPosition(futureTime);
        //
        //                    box.Position = new Vector2(futurePos.X - radius, futurePos.Y - radius);
        //
        //                    if (grid.HitDetectionResults(box))
        //                        nearby.Add(box);
        //
        //                    break;
        //            }
        //        }
        //    }
        //
        //    if (Vector2.Distance(Position, target) <= 32 && !nearby.Any()) return;
        //
        //    // iterate through grid tiles near the player and search projectiles for collisions
        //    for (int x = playerTile.X - view; x < playerTile.X + view; x++)
        //    {
        //        if (x < 0 || x >= gridDivisorWidth) continue;
        //        int tileX = x - gridDivisorWidth / 2;
        //
        //        for (int y = playerTile.Y - view; y < playerTile.Y + view; y++)
        //        {
        //            if (y < 0 || y >= gridDivisorHeight) continue;
        //            int tileY = y - gridDivisorHeight / 2;
        //
        //            //Tile hitbox
        //            RectangularHitbox tile = new()
        //            {
        //                Size = new Vector2(tileWidth, tileHeight),
        //                Position = new Vector2(tileWidth * tileX, tileHeight * tileY),
        //            };
        //
        //            //now check if any projectiles are intersecting this tile
        //            foreach (RectangularHitbox projectile in nearby)
        //            {
        //                //if the projectile is here increase the tile's "density" so we know to avoid it
        //                if (tile.HitDetectionResults(projectile))
        //                    tiles[x, y]++;
        //            }
        //        }
        //    }
        //
        //    //ok now that we have densities of the grid tiles,
        //    //lets choose a location to travel to thats close and safe
        //
        //    //The targeted tile
        //    Vector2Int targetTile = Vector2Int.Zero;
        //
        //    //just use TargetLocation for now
        //    for (int x = 0; x < gridDivisorWidth; x++)
        //    {
        //        int tileX = x - gridDivisorWidth / 2;
        //
        //        for (int y = 0; y < gridDivisorHeight; y++)
        //        {
        //            int tileY = y - gridDivisorHeight / 2;
        //
        //            //check if the TargetPosition is here
        //            if (target.X >= tileX * tileWidth && target.X <= (tileX + 1) * tileWidth &&
        //                target.Y >= tileY * tileHeight && target.Y <= (tileY + 1) * tileHeight)
        //                targetTile = new Vector2Int(x, y);
        //        }
        //    }
        //
        //    Vector2 targetTilePos = new(targetTile.X * tileWidth + (tileWidth / 2) - (playfield.X / 2),
        //        targetTile.Y * tileHeight + (tileHeight / 2) - (playfield.Y / 2));
        //    //Target.Position = targetTilePos;
        //
        //    //ok now that we have picked a location lets find a safe path to get there
        //
        //    //for now lets just pick a safe direction and assume the "path" is safe
        //
        //    List<Vector3Int> path = new();
        //    Vector3Int next = nextTile(playerTile, targetTile);
        //
        //    for (int i = 0; i < 4; i++)
        //    {
        //        path.Add(next);
        //
        //        if (next.XY == targetTile) break;
        //        next = nextTile(next.XY, targetTile);
        //    }
        //
        //    //visualize path a bit
        //    //for (int i = 0; i < Path.Count; i++)
        //    //    Path[i].Alpha = 0;
        //
        //    //for (int i = 1; i < path.Count && i < Path.Count; i++)
        //    //{
        //    //    Sprite node = Path[i - 1];
        //    //
        //    //    node.Position = new Vector2(path[i].X * tileWidth + (tileWidth / 2) - playfield.X / 2, path[i].Y * tileHeight + (tileHeight / 2) - playfield.Y / 2);
        //    //    node.Alpha = 0.75f;
        //    //}
        //
        //    //now lets go there!
        //    Vector3Int first = path.First();
        //
        //    Vector2 nextTilePos = new(first.X * tileWidth + (tileWidth / 2) - playfield.X / 2,
        //        first.Y * tileHeight + (tileHeight / 2) - playfield.Y / 2);
        //    //Safe.Position = nextTilePos;
        //
        //    //move X?
        //    if (Position.X < nextTilePos.X - tilePositioningMargin)
        //        AIBinds[VitaruActions.Right] = true;
        //    if (Position.X > nextTilePos.X + tilePositioningMargin)
        //        AIBinds[VitaruActions.Left] = true;
        //
        //    //move Y?
        //    if (Position.Y < nextTilePos.Y - tilePositioningMargin)
        //        AIBinds[VitaruActions.Down] = true;
        //    if (Position.Y > nextTilePos.Y + tilePositioningMargin)
        //        AIBinds[VitaruActions.Up] = true;
        //
        //    Vector3Int nextTile(Vector2Int current, Vector2Int final)
        //    {
        //        List<KeyValuePair<Vector2Int, float>> adjacent = adjacentTiles(current, final);
        //
        //        int density = 0;
        //        //if there are ever more than 128 projectiles on one tile then may god have no mercy for the level designer's soul...
        //        while (density <= 128)
        //        {
        //            for (int i = 0; i < adjacent.Count; i++)
        //            {
        //                Vector2Int ad = adjacent[i].Key;
        //                if (tiles[ad.X, ad.Y] <= density)
        //                    return new Vector3Int(ad, density);
        //            }
        //
        //            density++;
        //        }
        //
        //        return new Vector3Int(-1);
        //    }
        //
        //    //get adjacent tiles ordered by least total travel distance to it then the target
        //    List<KeyValuePair<Vector2Int, float>> adjacentTiles(Vector2Int current, Vector2Int final)
        //    {
        //        List<KeyValuePair<Vector2Int, float>> adjacent = new();
        //
        //        Vector2 finalTile = new(final.X * tileWidth + (tileWidth / 2) - playfield.X / 2,
        //            final.Y * tileHeight + (tileHeight / 2) - playfield.Y / 2);
        //
        //        for (int x = current.X - 1; x <= current.X + 1; x++)
        //        {
        //            if (x < 0 || x >= gridDivisorWidth || x == current.X) continue;
        //
        //            for (int y = current.Y - 1; y <= current.Y + 1; y++)
        //            {
        //                if (y < 0 || y >= gridDivisorHeight || y == current.Y) continue;
        //
        //                Vector2 adjacentTile = new(x * tileWidth + (tileWidth / 2) - playfield.X / 2,
        //                    y * tileHeight + (tileHeight / 2) - playfield.Y / 2);
        //                float travel = Vector2.Distance(Position, adjacentTile);
        //                float remaining = Vector2.Distance(adjacentTile, finalTile);
        //
        //                adjacent.Add(new KeyValuePair<Vector2Int, float>(new Vector2Int(x, y), travel + remaining));
        //            }
        //        }
        //
        //        return adjacent.OrderBy(pair => pair.Value).ToList();
        //    }
        //}


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
        [Description("Offense")] Offense,
        [Description("Defense")] Defense,
        [Description("Support")] Support,
        [Description("Specialized")] Specialized,
    }

    public enum Difficulty
    {
        [Description("Very Easy")] VeryEasy,
        [Description("Easy")] Easy,
        [Description("Normal")] Normal,
        [Description("Hard")] Hard,
        [Description("Very Hard")] VeryHard,
        [Description("Extreme")] Extreme,
        [Description("Impossible")] Impossible,
    }

    public enum ImplementationState
    {
        [Description("Not Functioning at All")] NotFunctioning,
        [Description("Partially Complete")] PartiallyComplete,
        [Description("Mostly Complete")] MostlyComplete,
        [Description("Complete")] Complete,
    }
}