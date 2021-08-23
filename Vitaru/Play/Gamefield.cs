// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Concurrent;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Utilities;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Groups.Packs;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Graphics.Particles;
using Vitaru.Graphics.Projectiles.Bullets;
using Vitaru.Levels;
using Vitaru.Networking.Client;
using Vitaru.Play.Characters;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;
using Vitaru.Settings;

namespace Vitaru.Play
{
    public class Gamefield : Pack<IPack>
    {
        public override string Name { get; set; } = nameof(Gamefield);

        public static double Current { get; private set; }

        public static double LastElapsedTime { get; private set; }

        private readonly bool multithread = Vitaru.VitaruSettings.GetBool(VitaruSetting.Multithreading);

        private readonly int particle_cap = Vitaru.VitaruSettings.GetInt(VitaruSetting.ParticleCap);

        public virtual Shades Shade { get; set; }

        public virtual float Intensity { get; set; } = 1;

        public int[] Indexes =
        {
            0,
            0
        };

        protected readonly FormatConverter FormatConverter;

        public readonly Pack<Character> PlayerPack = new()
        {
            Name = "Player Pack"
        };

        public readonly Layer2D<DrawableGameEntity> CharacterLayer = new()
        {
            Name = "Drawable Character Layer2D",
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize()
        };

        public readonly List<Enemy> UnloadedEnemies = new();

        public readonly Pack<Enemy> LoadedEnemies = new()
        {
            Name = "Loaded Enemies Pack"
        };

        public readonly List<ProjectilePack> ProjectilePacks = new();

        public readonly BulletLayer BulletLayer = new()
        {
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize()
        };

        public readonly ParticleLayer ParticleLayer = new()
        {
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize()
        };

        /// <summary>
        /// A layer for custom players to add stuff to
        /// </summary>
        public readonly Layer2D<IDrawable2D> OverlaysLayer = new()
        {
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize()
        };

        public readonly GamefieldBorder Border;

        private readonly ProjectilePack enemys;

        public Gamefield(VitaruNetHandler vitaruNet = null)
        {
            Add(PlayerPack);
            Add(LoadedEnemies);

            enemys = new ProjectilePack(this)
            {
                Name = "Enemy's Projectile Pack",
                Team = Enemy.ENEMY_TEAM,
                MultiThreading = multithread
            };

            if (multithread) AssignTasks(enemys);

            ProjectilePack players = new(this)
            {
                Name = "Player's Projectile Pack",
                Team = Player.PLAYER_TEAM
            };

            ProjectilePacks.Add(enemys);
            ProjectilePacks.Add(players);

            Add(enemys);
            Add(players);

            Border = new GamefieldBorder(GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize());

            if (vitaruNet != null)
            {
                //TODO: Multiplayer
            }

            OverlaysLayer.Clock = TrackManager.CurrentTrack.DrawClock;
            ParticleLayer.Clock = TrackManager.CurrentTrack.DrawClock;
            CharacterLayer.Clock = TrackManager.CurrentTrack.DrawClock;
            BulletLayer.Clock = TrackManager.CurrentTrack.DrawClock;
            Border.Clock = TrackManager.CurrentTrack.DrawClock;

            FormatConverter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();
            FormatConverter.Gamefield = this;

            try
            {
                if (LevelStore.CurrentLevel.EnemyData != null)
                    UnloadedEnemies.AddRange(FormatConverter.StringToEnemies(LevelStore.CurrentLevel.EnemyData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Enemies, purging bad data...", LogType.IO);
                UnloadedEnemies.Clear();
                LevelStore.CurrentLevel.EnemyData = String.Empty;
            }
        }

        private Benchmark waiting = new Benchmark("Waiting");

        public override void Update()
        {
            //Wait before we update Characters, that will mess this up
            
            waiting.Start();
            while (Vitaru.ThreadsRunning())
            {
            }
            waiting.Record();

            base.Update();

            Current = Clock.Current;
            LastElapsedTime = Clock.LastElapsedTime;

            while (deadprojectileQue.TryDequeue(out Projectile p))
            {
                Debugger.Assert(!p.Disposed,
                    $"Disposed {nameof(Projectile)}s shouldn't be in the {nameof(deadprojectileQue)}!");

                BulletLayer.ReturnIndex(p.Drawable);
                p.SetDrawable(-1, null);

                ProjectilePacks[p.Team].Remove(p);
            }

            if (multithread)
            {
                AssignIndexes(enemys);
                Vitaru.RunThreads();
            }
            else
                ParticleLayer.UpdateParticles(0, particle_cap, (float) Clock.LastElapsedTime);

            //should be safe to kill them from here
            while (deadEnemyQue.TryDequeue(out Enemy e))
            {
                Debugger.Assert(!e.Disposed, $"Disposed {nameof(Enemy)}s shouldn't be in the {nameof(deadEnemyQue)}!");
                LoadedEnemies.Remove(e, false);
                UnloadedEnemies.Add(e);
            }

            while (deadPlayerQue.TryDequeue(out Player p))
            {
                Debugger.Assert(!p.Disposed, $"Disposed {nameof(Player)}s shouldn't be in the {nameof(deadPlayerQue)}!");
                PlayerPack.Remove(p);
            }

            double current = Clock.Current;
            //Lets check our unloaded Enemies to see if any need to be drawn soon, if so lets load their drawables
            for (int i = 0; i < UnloadedEnemies.Count; i++)
            {
                Enemy e = UnloadedEnemies[i];
                if (current >= e.StartTime - e.TimePreLoad && current < e.EndTime) // + e.TimeUnLoad)
                {
                    enemyQue.Enqueue(e);
                    UnloadedEnemies.Remove(e);
                    LoadedEnemies.Add(e);
                    //Boss?.Enemies.Add(e);
                }
            }
        }

        private readonly ConcurrentQueue<Enemy> enemyQue = new();

        private readonly ConcurrentQueue<Enemy> deadEnemyQue = new();

        private readonly ConcurrentQueue<DrawableGameEntity> drawableCharacterQue =
            new();

        public void Add(Enemy enemy)
        {
            //We may not need to draw these yet so just store them in a list for now
            UnloadedEnemies.Add(enemy);
            enemy.OnAddParticle = ParticleLayer.Add;
        }

        public void Remove(Enemy enemy)
        {
            Debugger.Assert(!enemy.Disposed,
                $"Disposed {nameof(Enemy)}s shouldn't be getting added to {nameof(deadEnemyQue)}!");
            Debugger.Assert(!deadEnemyQue.Contains(enemy),
                $"{nameof(Enemy)} shouldn't be getting added to {nameof(deadEnemyQue)} again!");
            //que them since we may be calling this from their update loop
            deadEnemyQue.Enqueue(enemy);
        }

        private readonly ConcurrentQueue<Player> playerQue = new();

        private readonly ConcurrentQueue<Player> deadPlayerQue = new();

        public void Add(Player player)
        {
            PlayerPack.Add(player);
            player.OnAddParticle = ParticleLayer.Add;
            //Que adding the drawable
            playerQue.Enqueue(player);
        }

        public void Remove(Player player)
        {
            Debugger.Assert(!player.Disposed,
                $"Disposed {nameof(Player)}s shouldn't be getting added to {nameof(deadPlayerQue)}!");
            Debugger.Assert(!deadPlayerQue.Contains(player),
                $"{nameof(Player)} shouldn't be getting added to {nameof(deadPlayerQue)} again!");
            //que them since we may be calling this from their update loop
            deadPlayerQue.Enqueue(player);
            drawableCharacterQue.Enqueue(player.DrawablePlayer);
        }

        private readonly ConcurrentQueue<Projectile> deadprojectileQue = new();

        public void Add(Projectile projectile)
        {
            ProjectilePacks[projectile.Team].Add(projectile);
            //projectile.OnUnLoad += () => Remove(projectile);
        }

        public void Remove(Projectile projectile)
        {
            Debugger.Assert(!projectile.Disposed,
                $"Disposed {nameof(Projectile)}s shouldn't be getting added to {nameof(deadprojectileQue)}!");
            Debugger.Assert(!deadprojectileQue.Contains(projectile),
                $"{nameof(Projectile)} shouldn't be getting added to {nameof(deadprojectileQue)} again!");

            deadprojectileQue.Enqueue(projectile);
        }

        //Move the drawables on the draw thread to avoid threadsaftey issues
        public void PreRender()
        {
            //Add Players
            while (playerQue.TryDequeue(out Player p))
            {
                Debugger.Assert(!p.Disposed, "This player is disposed and should not be in this list anymore");
                DrawableGameEntity draw = p.GenerateDrawable();
                p.SetDrawable(draw);

                draw.OnDelete += () => drawableCharacterQue.Enqueue(draw);

                CharacterLayer.Add(draw);
            }

            //Add Enemies
            while (enemyQue.TryDequeue(out Enemy e))
            {
                Debugger.Assert(!e.Disposed, "This enemy is disposed and should not be in this list anymore");
                DrawableGameEntity draw = e.GenerateDrawable();
                e.SetDrawable(draw);

                draw.OnDelete += () => drawableCharacterQue.Enqueue(draw);

                CharacterLayer.Add(draw);
            }

            //Remove Characters
            while (drawableCharacterQue.TryDequeue(out DrawableGameEntity draw))
            {
                CharacterLayer.Remove(draw);
            }
        }

        //Assign the multithreads their tasks
        public void AssignTasks(ProjectilePack pack)
        {
            for (int i = 0; i < Vitaru.DynamicThreads.Count; i++)
            {
                int s = i;
                int e = i + Vitaru.DynamicThreads.Count;

                Vitaru.DynamicThreads[i].Task = () =>
                {
                    if (pack.Children.Count > 0)
                        pack.ProcessBullets(s, e);
                    processParticles(s, e);
                };
            }

            int tCount = Vitaru.DynamicThreads.Count;
            int pCount = particle_cap;

            Indexes = new int[tCount * 2];

            //Amount of particles per thread
            int[] count = PrionMath.DistributeInteger(pCount, tCount).ToArray();
            int roll = 0;

            for (int i = 0; i < tCount; i++)
            {
                int s = roll;
                roll += count[i];
                int e = roll;

                if (count[i] == 0)
                {
                    s = 0;
                    e = 0;
                }

                Indexes[i] = s;
                Indexes[i + tCount] = e;
            }
        }

        public void AssignIndexes(ProjectilePack pack)
        {
            int tCount = Vitaru.DynamicThreads.Count;
            int pCount = pack.Children.Count;

            pack.Indexes = new int[tCount * 2];

            //Amount of bullets per thread
            int[] count = PrionMath.DistributeInteger(pCount, tCount).ToArray();
            int roll = 0;

            for (int i = 0; i < tCount; i++)
            {
                int s = roll;
                roll += count[i];
                int e = roll;

                if (count[i] == 0)
                {
                    s = 0;
                    e = 0;
                }

                pack.Indexes[i] = s;
                pack.Indexes[i + tCount] = e;
            }
        }

        private void processParticles(int s, int e)
        {
            int start = Indexes[s];
            int end = Indexes[e];

            ParticleLayer.UpdateParticles(start, end, (float) Clock.LastElapsedTime);
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Logger.Benchmark(waiting);
        }

        public class ProjectilePack : Pack<Projectile>, IHasTeam
        {
            public int Team { get; set; }

            public int[] Indexes =
            {
                0,
                0
            };

            public bool MultiThreading { get; set; }

            private readonly Gamefield gamefield;

            public ProjectilePack(Gamefield gamefield)
            {
                this.gamefield = gamefield;
            }

            public override void Update()
            {
                if (!MultiThreading) ProcessBullets(0, 1);
            }

            public override void Add(Projectile child, AddPosition position = AddPosition.Last)
            {
                base.Add(child, position);
                if (!MultiThreading) Indexes[1] = ProtectedChildren.Count;
            }

            public override void Remove(Projectile child, bool dispose = true)
            {
                base.Remove(child, dispose);
                if (!MultiThreading) Indexes[1] = ProtectedChildren.Count;
            }

            public void ProcessBullets(int s, int e)
            {
                Random r = new();
                int start = Indexes[s];
                int end = Indexes[e];

                for (int i = start; i < end; i++)
                {
                    Projectile p = ProtectedChildren[i];

                    if (Current + p.TimePreLoad >= p.StartTime && Current < p.EndTime + p.TimeUnLoad && !p.PreLoaded)
                    {
                        p.SetDrawable(gamefield.BulletLayer.RequestIndex(), gamefield.BulletLayer);
                        p.OnAddParticle = gamefield.ParticleLayer.Add;

                        p.PreLoad();
                    }
                    else if ((Current + p.TimePreLoad < p.StartTime || Current >= p.EndTime + p.TimeUnLoad) &&
                             p.PreLoaded)
                    {
                        p.UnLoad();
                        gamefield.Remove(p);
                    }

                    if (Current >= p.StartTime && Current < p.EndTime && !p.Started)
                        p.Start();
                    else if ((Current < p.StartTime || Current >= p.EndTime) && p.Started)
                    {
                        p.End();
                    }

                    p.ConcurrentUpdate(r);
                }
            }
        }

        public class GamefieldBorder : Layer2D<Box>
        {
            public GamefieldBorder(Vector2 size)
            {
                Size = size;

                const int w = 2;
                Children = new[]
                {
                    new Box
                    {
                        Height = w,
                        Width = size.X,
                        Y = -size.Y / 2
                    },
                    new Box
                    {
                        Height = w,
                        Width = size.X,
                        Y = size.Y / 2
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y,
                        X = -size.X / 2
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y,
                        X = size.X / 2
                    }
                };
            }
        }
    }
}