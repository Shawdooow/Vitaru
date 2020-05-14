﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Layers;
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

        public readonly Layer2D<DrawableCharacter> CharacterLayer = new Layer2D<DrawableCharacter>
        {
            Name = "Drawable Character Layer2D"
        };

        protected readonly List<Enemy> UnloadedEnemies = new List<Enemy>();

        public readonly Pack<Character> LoadedEnemies = new Pack<Character>
        {
            Name = "Loaded Enemies Pack"
        };

        public readonly Dictionary<int, Pack<Projectile>> ProjectilePacks = new Dictionary<int, Pack<Projectile>>();

        public readonly Layer2D<DrawableProjectile> ProjectileLayer = new Layer2D<DrawableProjectile>
        {
            Name = "Drawable Projectile Layer2D"
        };

        public Gamefield(VitaruNetHandler vitaruNet = null)
        {
            Add(PlayerPack);
            Add(LoadedEnemies);

            Pack<Projectile> enemys = new Pack<Projectile>()
            {
                Name = "Enemy's Projectile Pack"
            };

            Pack<Projectile> players = new Pack<Projectile>()
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
                if (Clock.Current + p.TimePreLoad >= p.StartTime && Clock.Current < p.EndTime + p.TimeUnLoad &&
                    !p.PreLoaded)
                    p.PreLoad();
                else if ((Clock.Current + p.TimePreLoad < p.StartTime || Clock.Current >= p.EndTime + p.TimeUnLoad) &&
                         p.PreLoaded)
                {
                    p.UnLoad();
                    Remove(p);
                }

                if (Clock.Current >= p.StartTime && Clock.Current < p.EndTime && !p.Started)
                    p.Start();
                else if ((Clock.Current < p.StartTime || Clock.Current >= p.EndTime) && p.Started)
                    p.End();
            }

            //Lets check our unloaded Enemies to see if any need to be drawn soon, if so lets load their drawables
            for (int i = 0; i < UnloadedEnemies.Count; i++)
            {
                Enemy e = UnloadedEnemies[i];
                if (Clock.Current >= e.StartTime - e.TimePreLoad && Clock.Current < e.EndTime) // + e.TimeUnLoad)
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

        private readonly List<DrawableEnemy> drawableEnemyQue = new List<DrawableEnemy>();

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

        private readonly List<Player> playerQue = new List<Player>();

        public void Add(Player player)
        {
            PlayerPack.Add(player);
            //Que adding the drawable
            playerQue.Add(player);
        }

        private readonly List<Projectile> projectileQue = new List<Projectile>();

        private readonly List<Projectile> deadprojectileQue = new List<Projectile>();

        private readonly List<DrawableProjectile> drawableProjectileQue = new List<DrawableProjectile>();

        public void Add(Projectile projectile)
        {
            ProjectilePacks[projectile.Team].Add(projectile);
            //projectile.OnUnLoad += () => Remove(projectile);
            //Que adding the drawable
            projectileQue.Add(projectile);
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
                CharacterLayer.Add(playerQue[0].GenerateDrawable());
                playerQue.Remove(playerQue[0]);
            }

            //Add / Remove Enemies
            while (enemyQue.Count > 0)
            {
                Enemy enemy = enemyQue[0];
                DrawableEnemy draw = enemy.GenerateDrawable();

                draw.OnDelete += () => drawableEnemyQue.Add(draw);

                CharacterLayer.Add(draw);
                enemyQue.Remove(enemy);
            }

            while (drawableEnemyQue.Count > 0)
            {
                DrawableEnemy draw = drawableEnemyQue[0];
                CharacterLayer.Remove(draw);
                drawableEnemyQue.Remove(draw);
            }

            //Add / Remove Projectiles
            while (projectileQue.Count > 0)
            {
                Projectile projectile = projectileQue[0];
                DrawableProjectile draw = projectile.GenerateDrawable();

                draw.OnDelete += () => drawableProjectileQue.Add(draw);

                ProjectileLayer.Add(draw);
                projectileQue.Remove(projectile);
            }

            while (drawableProjectileQue.Count > 0)
            {
                DrawableProjectile draw = drawableProjectileQue[0];
                ProjectileLayer.Remove(draw);
                drawableProjectileQue.Remove(draw);
            }
        }
    }
}