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
            if (vitaruNet != null)
            {
                //TODO: Multiplayer
            }
        }

        public override void Update()
        {
            //Lets check our unloaded Enemies to see if any need to be drawn soon, if so lets load their drawables
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

        private readonly List<DrawableEnemy> drawableEnemyQue = new List<DrawableEnemy>();

        public void Add(Enemy enemy)
        {
            //We may not need to draw these yet so just store them in a list for now
            UnloadedEnemies.Add(enemy);
        }

        public void Remove(Enemy enemy)
        {
            LoadedEnemies.Remove(enemy, false);
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

        private readonly List<DrawableProjectile> drawableProjectileQue = new List<DrawableProjectile>();

        public void Add(Projectile projectile)
        {
            ProjectilePack.Add(projectile);
            //Que adding the drawable
            projectileQue.Add(projectile);
        }

        public void Remove(Projectile projectile)
        {
            projectile.Delete();
            ProjectilePack.Remove(projectile);
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