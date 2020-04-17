// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Layers;
using Vitaru.Characters;
using Vitaru.Characters.Enemies;
using Vitaru.Characters.Players;
using Vitaru.Multiplayer.Client;
using Vitaru.Projectiles;

namespace Vitaru.Play
{
    public class Gamefield : Pack<IPack>
    {
        public readonly Pack<Character> PlayerPack = new Pack<Character>();

        public readonly SpriteLayer<DrawableCharacter> CharacterLayer = new SpriteLayer<DrawableCharacter>();

        protected readonly List<Enemy> UnloadedEnemies = new List<Enemy>();

        public readonly Pack<Character> LoadedEnemies = new Pack<Character>();

        public readonly Pack<Projectile> ProjectilePack = new Pack<Projectile>();

        public readonly SpriteLayer<DrawableProjectile> ProjectileLayer = new SpriteLayer<DrawableProjectile>();

        public Gamefield(VitaruNetHandler vitaruNet = null)
        {
            //TODO: Multiplayer
        }

        public override void Update()
        {
            for (int i = 0; i < UnloadedEnemies.Count; i++)
            {
                Enemy e = UnloadedEnemies[i];
                if (Clock.Current >= e.StartTime - e.TimePreLoad && Clock.Current < e.EndTime + e.TimeUnLoad)
                {
                    enemyQue.Add(e);
                    UnloadedEnemies.Remove(e);
                    LoadedEnemies.Add(e);
                    //Boss?.Enemies.Add(e);
                }
            }
        }

        private readonly List<Enemy> enemyQue = new List<Enemy>();

        public void Add(Enemy enemy)
        {
            UnloadedEnemies.Add(enemy);
        }

        private readonly List<Player> playerQue = new List<Player>();

        public void Add(Player player)
        {
            PlayerPack.Add(player);
            //Que adding the drawable
            playerQue.Add(player);
        }

        private readonly List<Projectile> projectileQue = new List<Projectile>();

        public void Add(Projectile projectile)
        {
            ProjectilePack.Add(projectile);
            //Que adding the drawable
            projectileQue.Add(projectile);
        }

        //Add the drawable on the draw thread to avoid threadsaftey issues
        public void PreRender()
        {
            while (playerQue.Count > 0)
            {
                CharacterLayer.Add(playerQue[0].GenerateDrawable());
                playerQue.Remove(playerQue[0]);
            }

            while (enemyQue.Count > 0)
            {
                Enemy enemy = enemyQue[0];
                DrawableEnemy draw = enemy.GenerateDrawable();

                draw.OnDispose += () =>
                {
                    LoadedEnemies.Remove(enemy);
                    UnloadedEnemies.Add(enemy);
                };

                CharacterLayer.Add(draw);
                enemyQue.Remove(enemy);
            }

            while (projectileQue.Count > 0)
            {
                ProjectileLayer.Add(projectileQue[0].GenerateDrawable());
                projectileQue.Remove(projectileQue[0]);
            }
        }

        protected override void Dispose(bool finalize)
        {
            PlayerPack?.Dispose();
            CharacterLayer?.Dispose();
            LoadedEnemies?.Dispose();
            ProjectilePack?.Dispose();
            ProjectileLayer?.Dispose();
            base.Dispose(finalize);
        }
    }
}