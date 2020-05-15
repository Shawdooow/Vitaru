// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Concurrent;
using System.Collections.Generic;
using Prion.Application.Debug;
using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Layers;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Multiplayer.Client;

namespace Vitaru.Play
{
    public class Gamefield : Pack<IPack>
    {
        public override string Name { get; set; } = nameof(Gamefield);

        public readonly Pack<Character> PlayerPack = new Pack<Character>
        {
            Name = "Player Pack"
        };

        public readonly Layer2D<DrawableGameEntity> CharacterLayer = new Layer2D<DrawableGameEntity>
        {
            Name = "Drawable Character Layer2D"
        };

        protected readonly List<Enemy> UnloadedEnemies = new List<Enemy>();

        public readonly Pack<Character> LoadedEnemies = new Pack<Character>
        {
            Name = "Loaded Enemies Pack"
        };

        public readonly Dictionary<int, Pack<Projectile>> ProjectilePacks = new Dictionary<int, Pack<Projectile>>();

        public readonly Layer2D<DrawableGameEntity> ProjectileLayer = new Layer2D<DrawableGameEntity>
        {
            Name = "Drawable Projectile Layer2D"
        };

        protected readonly Queue<DrawableProjectile> RecycledDrawableProjectiles = new Queue<DrawableProjectile>();

        public Gamefield(VitaruNetHandler vitaruNet = null)
        {
            Add(PlayerPack);
            Add(LoadedEnemies);

            Pack<Projectile> enemys = new Pack<Projectile>
            {
                Name = "Enemy's Projectile Pack"
            };

            Pack<Projectile> players = new Pack<Projectile>
            {
                Name = "Enemy's Projectile Pack"
            };

            ProjectilePacks.Add(Enemy.ENEMY_TEAM, enemys);
            ProjectilePacks.Add(Player.PLAYER_TEAM, players);

            Add(enemys);
            Add(players);

            if (vitaruNet != null)
            {
                //TODO: Multiplayer
            }
        }

        public override void Update()
        {
            base.Update();

            //should be safe to kill them from here
            while (deadEnemyQue.Count > 0)
            {
                Enemy enemy = deadEnemyQue[0];
                deadEnemyQue.Remove(enemy);
                LoadedEnemies.Remove(enemy, false);
                UnloadedEnemies.Add(enemy);
            }

            while (deadprojectileQue.Count > 0)
            {
                Projectile projectile = deadprojectileQue[0];
                deadprojectileQue.Remove(projectile);
                ProjectilePacks[projectile.Team].Remove(projectile);
            }

            foreach (KeyValuePair<int, Pack<Projectile>> pair in ProjectilePacks)
            foreach (Projectile p in pair.Value)
            {
                if (Clock.LastCurrent + p.TimePreLoad >= p.StartTime && Clock.LastCurrent < p.EndTime + p.TimeUnLoad &&
                    !p.PreLoaded)
                    p.PreLoad();
                else if ((Clock.LastCurrent + p.TimePreLoad < p.StartTime || Clock.LastCurrent >= p.EndTime + p.TimeUnLoad) &&
                         p.PreLoaded)
                {
                    p.UnLoad();
                    Remove(p);
                }

                if (Clock.LastCurrent >= p.StartTime && Clock.LastCurrent < p.EndTime && !p.Started)
                    p.Start();
                else if ((Clock.LastCurrent < p.StartTime || Clock.LastCurrent >= p.EndTime) && p.Started)
                    p.End();
            }

            //Lets check our unloaded Enemies to see if any need to be drawn soon, if so lets load their drawables
            for (int i = 0; i < UnloadedEnemies.Count; i++)
            {
                Enemy e = UnloadedEnemies[i];
                if (Clock.LastCurrent >= e.StartTime - e.TimePreLoad && Clock.LastCurrent < e.EndTime) // + e.TimeUnLoad)
                {
                    enemyQue.Add(e);
                    UnloadedEnemies.Remove(e);
                    LoadedEnemies.Add(e);
                    //Boss?.Enemies.Add(e);
                }
            }
        }

        private readonly List<Enemy> enemyQue = new List<Enemy>();

        private readonly List<Enemy> deadEnemyQue = new List<Enemy>();

        private readonly List<DrawableGameEntity> drawableEnemyQue = new List<DrawableGameEntity>();

        public void Add(Enemy enemy)
        {
            //We may not need to draw these yet so just store them in a list for now
            UnloadedEnemies.Add(enemy);
        }

        public void Remove(Enemy enemy)
        {
            //que them since we may be calling this from their update loop
            deadEnemyQue.Add(enemy);
        }

        private readonly List<DrawableGameEntity> playerQue = new List<DrawableGameEntity>();

        public void Add(Player player)
        {
            PlayerPack.Add(player);
            //Que adding the drawable

            DrawableGameEntity draw = player.GenerateDrawable();
            player.SetDrawable(draw);
            playerQue.Add(draw);
        }

        //TODO: evaluate performance loss of this
        private readonly ConcurrentQueue<DrawableProjectile> projectileQue = new ConcurrentQueue<DrawableProjectile>();

        private readonly List<Projectile> deadprojectileQue = new List<Projectile>();

        private readonly Queue<DrawableProjectile> drawableProjectileQue = new Queue<DrawableProjectile>();

        public void Add(Projectile projectile)
        {
            ProjectilePacks[projectile.Team].Add(projectile);
            //projectile.OnUnLoad += () => Remove(projectile);

            DrawableProjectile draw;

            if (RecycledDrawableProjectiles.Count > 0)
                draw = RecycledDrawableProjectiles.Dequeue();
            else
                draw = projectile.GenerateDrawable() as DrawableProjectile;

            projectile.SetDrawable(draw);
            draw.SetProjectile(projectile);

            //Que adding the drawable
            projectileQue.Enqueue(draw);
        }

        public void Remove(Projectile projectile)
        {
            projectile.Delete();
            deadprojectileQue.Add(projectile);
        }

        //Move the drawables on the draw thread to avoid threadsaftey issues
        public void PreRender()
        {
            //Add Players
            while (playerQue.Count > 0)
            {
                CharacterLayer.Add(playerQue[0]);
                playerQue.Remove(playerQue[0]);
            }

            //Add / Remove Enemies
            while (enemyQue.Count > 0)
            {
                Enemy enemy = enemyQue[0];
                PrionDebugger.Assert(!enemy.Disposed, "This enemy is disposed and should not be in this list anymore");
                DrawableGameEntity draw = enemy.GenerateDrawable();
                enemy.SetDrawable(draw);

                draw.OnDelete += () => drawableEnemyQue.Add(draw);

                CharacterLayer.Add(draw);
                enemyQue.Remove(enemy);
            }

            while (drawableEnemyQue.Count > 0)
            {
                DrawableGameEntity draw = drawableEnemyQue[0];
                CharacterLayer.Remove(draw);
                drawableEnemyQue.Remove(draw);
            }

            //Add / Remove Projectiles
            if (projectileQue.Count > 0)
            {
                PrionDebugger.Assert(projectileQue.TryDequeue(out DrawableProjectile draw));
                PrionDebugger.Assert(!draw.Disposed, "This projectile is disposed and should not be in this list anymore");

                draw.OnDelete += () => drawableProjectileQue.Enqueue(draw);

                ProjectileLayer.Add(draw);
            }

            while (drawableProjectileQue.Count > 0)
            {
                DrawableProjectile draw = drawableProjectileQue.Dequeue();
                ProjectileLayer.Remove(draw, false);
                RecycledDrawableProjectiles.Enqueue(draw);
            }
        }
    }
}