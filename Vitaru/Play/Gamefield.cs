// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Prion.Golgi.Utilities;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Groups.Packs;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Characters.Players;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Graphics;
using Vitaru.Graphics.Particles;
using Vitaru.Multiplayer.Client;
using Vitaru.Settings;

namespace Vitaru.Play
{
    public class Gamefield : Pack<IPack>
    {
        public override string Name { get; set; } = nameof(Gamefield);

        public Action<Shades> OnShadeChange;

        public Action<float> OnIntensityChange;

        public Shades Shade
        {
            get => shade;
            set
            {
                shade = value;
                OnShadeChange?.Invoke(shade);
            }
        }

        private Shades shade;

        public float Intensity
        {
            get => intensity;
            set
            {
                intensity = value;
                OnIntensityChange?.Invoke(intensity);
            }
        }

        private float intensity;

        public readonly Pack<Character> PlayerPack = new Pack<Character>
        {
            Name = "Player Pack"
        };

        public readonly ShadeLayer<DrawableGameEntity> CharacterLayer = new ShadeLayer<DrawableGameEntity>
        {
            Name = "Drawable Character Layer2D"
        };

        protected readonly List<Enemy> UnloadedEnemies = new List<Enemy>();

        public readonly Pack<Enemy> LoadedEnemies = new Pack<Enemy>
        {
            Name = "Loaded Enemies Pack"
        };

        public readonly List<ProjectilePack> ProjectilePacks = new List<ProjectilePack>();

        public readonly ShadeLayer<DrawableGameEntity> ProjectilesLayer = new ProjectileLayer();

        protected readonly Queue<DrawableProjectile> RecycledDrawableProjectiles = new Queue<DrawableProjectile>();

        public readonly ParticleLayer ParticleLayer = new ParticleLayer();

        public Gamefield(VitaruNetHandler vitaruNet = null)
        {
            Add(PlayerPack);
            Add(LoadedEnemies);

            ProjectilePack enemys = new ProjectilePack(this)
            {
                Name = "Enemy's Projectile Pack",
                Team = Enemy.ENEMY_TEAM
            };

            ProjectilePack players = new ProjectilePack(this)
            {
                Name = "Player's Projectile Pack",
                Team = Player.PLAYER_TEAM
            };

            ProjectilePacks.Add(enemys);
            ProjectilePacks.Add(players);

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
                Debugger.Assert(deadEnemyQue.TryDequeue(out Enemy enemy));
                Debugger.Assert(!enemy.Disposed);
                LoadedEnemies.Remove(enemy, false);
                UnloadedEnemies.Add(enemy);
            }

            while (deadprojectileQue.Count > 0)
            {
                Debugger.Assert(deadprojectileQue.TryDequeue(out Projectile projectile));
                Debugger.Assert(!projectile.Disposed);
                ProjectilePacks[projectile.Team].Remove(projectile);
            }

            //Lets check our unloaded Enemies to see if any need to be drawn soon, if so lets load their drawables
            for (int i = 0; i < UnloadedEnemies.Count; i++)
            {
                Enemy e = UnloadedEnemies[i];
                if (Clock.LastCurrent >= e.StartTime - e.TimePreLoad && Clock.LastCurrent < e.EndTime
                ) // + e.TimeUnLoad)
                {
                    enemyQue.Enqueue(e);
                    UnloadedEnemies.Remove(e);
                    LoadedEnemies.Add(e);
                    //Boss?.Enemies.Add(e);
                }
            }
        }

        private readonly ConcurrentQueue<Enemy> enemyQue = new ConcurrentQueue<Enemy>();

        private readonly ConcurrentQueue<Enemy> deadEnemyQue = new ConcurrentQueue<Enemy>();

        private readonly ConcurrentQueue<DrawableGameEntity> drawableEnemyQue =
            new ConcurrentQueue<DrawableGameEntity>();

        public void Add(Enemy enemy)
        {
            //We may not need to draw these yet so just store them in a list for now
            UnloadedEnemies.Add(enemy);
        }

        public void Remove(Enemy enemy)
        {
            //que them since we may be calling this from their update loop
            deadEnemyQue.Enqueue(enemy);
        }

        private readonly ConcurrentQueue<Player> playerQue = new ConcurrentQueue<Player>();

        public void Add(Player player)
        {
            PlayerPack.Add(player);
            //Que adding the drawable
            playerQue.Enqueue(player);
        }

        private readonly ConcurrentQueue<DrawableProjectile> projectileQue = new ConcurrentQueue<DrawableProjectile>();

        private readonly ConcurrentQueue<Projectile> deadprojectileQue = new ConcurrentQueue<Projectile>();

        private readonly ConcurrentQueue<DrawableProjectile> drawableProjectileQue =
            new ConcurrentQueue<DrawableProjectile>();

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
            deadprojectileQue.Enqueue(projectile);
        }

        //Move the drawables on the draw thread to avoid threadsaftey issues
        public void PreRender()
        {
            //Add Players
            while (playerQue.Count > 0)
            {
                Debugger.Assert(playerQue.TryDequeue(out Player player));
                DrawableGameEntity draw = player.GenerateDrawable();
                player.SetDrawable(draw);
                CharacterLayer.Add(draw);
            }

            //Add / Remove Enemies
            while (enemyQue.Count > 0)
            {
                Debugger.Assert(enemyQue.TryDequeue(out Enemy enemy));
                Debugger.Assert(!enemy.Disposed, "This enemy is disposed and should not be in this list anymore");
                DrawableGameEntity draw = enemy.GenerateDrawable();
                enemy.SetDrawable(draw);

                draw.OnDelete += () => drawableEnemyQue.Enqueue(draw);

                CharacterLayer.Add(draw);
            }

            while (drawableEnemyQue.Count > 0)
            {
                Debugger.Assert(drawableEnemyQue.TryDequeue(out DrawableGameEntity draw));
                CharacterLayer.Remove(draw);
            }

            //Add / Remove Projectiles
            while (projectileQue.Count > 0)
            {
                Debugger.Assert(projectileQue.TryDequeue(out DrawableProjectile draw));
                Debugger.Assert(!draw.Disposed,
                    "This projectile is disposed and should not be in this list anymore");

                draw.OnDelete += () => drawableProjectileQue.Enqueue(draw);

                ProjectilesLayer.Add(draw);
            }

            while (drawableProjectileQue.Count > 0)
            {
                Debugger.Assert(drawableProjectileQue.TryDequeue(out DrawableProjectile draw));
                ProjectilesLayer.Remove(draw, false);
                RecycledDrawableProjectiles.Enqueue(draw);
            }
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);

            while (RecycledDrawableProjectiles.Count > 0)
                RecycledDrawableProjectiles.Dequeue().Dispose();
        }

        public class ProjectilePack : Pack<Projectile>, IHasTeam
        {
            public int Team { get; set; }

            private readonly List<List<Projectile>> lists = new List<List<Projectile>>();

            private bool threading;

            private readonly Gamefield gamefield;

            public ProjectilePack(Gamefield gamefield)
            {
                this.gamefield = gamefield;
            }

            public override void Update()
            {
                if (ProtectedChildren.Count >= 500 && !threading)
                    enableThreading();

                if (!threading)
                    proccessList(ProtectedChildren);
                else
                {
                    Vitaru.RunThreads();
                    proccessList(lists.Last());
                    Vitaru.AwaitDynamicThreads();
                }
            }

            public override void Add(Projectile child, AddPosition position = AddPosition.Last)
            {
                base.Add(child, position);

                if (threading)
                {
                    List<Projectile> smallest = lists[0];
                    for (int i = 1; i < lists.Count; i++)
                        if (lists[i].Count < smallest.Count)
                            smallest = lists[i];

                    smallest.Add(child);
                }
            }

            public override void Remove(Projectile child, bool dispose = true)
            {
                base.Remove(child, dispose);

                if (threading)
                {
                    for (int i = 0; i < lists.Count; i++)
                    {
                        if (lists[i].Contains(child))
                        {
                            lists[i].Remove(child);
                            break;
                        }
                    }
                }
            }

            private void proccessList(List<Projectile> list)
            {
                double last = Clock.LastCurrent;

                for (int i = 0; i < list.Count; i++)
                {
                    Projectile p = list[i];

                    if (last + p.TimePreLoad >= p.StartTime && last < p.EndTime + p.TimeUnLoad && !p.PreLoaded)
                        p.PreLoad();
                    else if ((last + p.TimePreLoad < p.StartTime || last >= p.EndTime + p.TimeUnLoad) && p.PreLoaded)
                    {
                        p.UnLoad();
                        gamefield.Remove(p);
                    }

                    if (last >= p.StartTime && last < p.EndTime && !p.Started)
                        p.Start();
                    else if ((last < p.StartTime || last >= p.EndTime) && p.Started)
                    {
                        p.End();
                    }

                    p.Update();
                }
            }

            private void enableThreading()
            {
                threading = true;

                for (int i = 0; i < Vitaru.Threads.Count; i++)
                {
                    List<Projectile> list = new List<Projectile>();
                    Vitaru.Threads[i].Task = () => proccessList(list);
                    lists.Add(list);
                }

                lists.Add(new List<Projectile>());

                int t = 0;

                for (int i = 0; i < ProtectedChildren.Count; i++)
                {
                    lists[t].Add(ProtectedChildren[i]);

                    t++;
                    if (t >= lists.Count)
                        t = 0;
                }
            }
        }

        private class ProjectileLayer : ShadeLayer<DrawableGameEntity>
        {
            private readonly Benchmark benchmark = new Benchmark("Bullet Render Time");

            private readonly GraphicsOptions graphics =
                Vitaru.VitaruSettings.GetValue<GraphicsOptions>(VitaruSetting.BulletVisuals);

            private readonly List<DrawableBullet> bullets = new List<DrawableBullet>();

            public ProjectileLayer()
            {
                Name = "Drawable Projectile Layer2D";
            }

            public override void Render()
            {
                benchmark.Start();

                switch (graphics)
                {
                    default:
                        base.Render();
                        break;
                    case GraphicsOptions.HighPerformance:
                        Renderer.ShaderManager.ActiveShaderProgram = Renderer.SpriteProgram;
                        Renderer.ShaderManager.UpdateInt("shade", (int) Shade);
                        Renderer.ShaderManager.UpdateFloat("intensity", Intensity);

                        //Draw Glows
                        Renderer.ShaderManager.UpdateVector3("spriteColor", bullets[0].Glow.Color.Vector());
                        Renderer.CurrentContext.BindTexture(bullets[0].Glow.Texture);
                        for (int i = 0; i < bullets.Count; i++)
                        {
                            Renderer.ShaderManager.UpdateMatrix4("model", bullets[i].Glow.DrawTransform);
                            Renderer.ShaderManager.UpdateVector2("size", bullets[i].Glow.Size);
                            Renderer.ShaderManager.UpdateFloat("alpha", bullets[i].Glow.DrawAlpha);
                            Renderer.CurrentContext.RenderSpriteQuad();
                        }

                        //Draw Whites
                        Renderer.ShaderManager.UpdateVector3("spriteColor", Color.White.Vector());
                        Renderer.CurrentContext.BindTexture(bullets[0].Center.Texture);
                        for (int i = 0; i < bullets.Count; i++)
                        {
                            Renderer.ShaderManager.UpdateMatrix4("model", bullets[i].Center.DrawTransform);
                            Renderer.ShaderManager.UpdateVector2("size", bullets[i].Center.Size);
                            Renderer.ShaderManager.UpdateFloat("alpha", bullets[i].Center.DrawAlpha);
                            Renderer.CurrentContext.RenderSpriteQuad();
                        }

                        Renderer.ShaderManager.UpdateInt("shade", 0);
                        Renderer.ShaderManager.UpdateFloat("intensity", 1);
                        break;
                }

                benchmark.Record();
            }

            public override void Add(DrawableGameEntity child, AddPosition position = AddPosition.Last)
            {
                if (graphics == GraphicsOptions.HighPerformance && child is DrawableBullet bullet)
                    bullets.Add(bullet);

                base.Add(child, position);
            }

            public override void Remove(DrawableGameEntity child, bool dispose = true)
            {
                if (graphics == GraphicsOptions.HighPerformance && child is DrawableBullet bullet)
                    bullets.Remove(bullet);

                base.Remove(child, dispose);
            }

            protected override void Dispose(bool finalize)
            {
                base.Dispose(finalize);
                Logger.Benchmark(benchmark);
            }
        }
    }
}