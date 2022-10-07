using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using System.Collections.Generic;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two;
using Vitaru.Gamemodes;
using Vitaru.Play.Characters;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.KeyFrames;
using Vitaru.Play.Teams;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Audio;
using Prion.Nucleus.Utilities;
using System.Collections.Concurrent;
using System.Drawing;
using System;
using Vitaru.Graphics.Particles;
using Vitaru.Graphics.Projectiles.Bullets;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Projectiles;
using Prion.Nucleus.Debug;
using Vitaru.Networking.Client;
using System.Numerics;
using Prion.Mitochondria.Graphics;

namespace Vitaru.Play
{
    public class PlayManager : Pack<IUpdatable>
    {
        public static double Current { get; private set; } = double.MinValue;
        public static double LastElapsedTime { get; private set; }

        public Player ActivePlayer { get; protected set; }

        public readonly List<TeamList> Teams = new List<TeamList>
        {
            new TeamList
            {
                Team = Player.PLAYER_TEAM,
            },
            new TeamList
            {
                Team = Player.PLAYER_TEAM + 1,
            },
            new TeamList
            {
                Team = Player.PLAYER_TEAM + 2,
            },
        };

        public readonly Pack<Character> CharacterPack = new()
        {
            Name = "Character Pack",
        };

        public readonly EnemyManager EnemyManager = new EnemyManager();
        public readonly ProjectileManager ProjectileManager = new ProjectileManager();

        public readonly PlayLayers Layers;

        protected float LastHealth;
        protected float LastEnergy;

        private double beat = 1000 / 60d;
        private double lastQuarterBeat = double.MinValue;
        private double nextHalfBeat = double.MinValue;
        private double nextQuarterBeat = double.MinValue;

        public PlayManager(PlayLayers layers, VitaruNetHandler vitaruNet = null)
        {
            Layers = layers;

            Player player = GamemodeStore.SelectedGamemode.SelectedCharacter != string.Empty
                ? GamemodeStore.GetPlayer(GamemodeStore.SelectedGamemode.SelectedCharacter, this)
                : new Yuie(this);
        }

        public override void PreLoading()
        {
            base.PreLoading();

            Add(CharacterPack);
            Add(EnemyManager);
            Add(ProjectileManager);
        }

        public virtual void OnNewBeat()
        {
            OnHalfBeat();

            beat = TrackManager.CurrentTrack.Metadata.GetBeatLength();

            lastQuarterBeat = Clock.LastCurrent;
            nextHalfBeat = Clock.LastCurrent + beat / 2;
            nextQuarterBeat = Clock.LastCurrent + beat / 4;

            foreach (Character c in CharacterPack)
                c.OnNewBeat();
        }

        protected virtual void OnHalfBeat()
        {
            OnQuarterBeat();
            nextHalfBeat = -1;
            foreach (Character c in CharacterPack)
                c.OnHalfBeat();
        }

        protected virtual void OnQuarterBeat()
        {
            lastQuarterBeat = nextQuarterBeat;
            nextQuarterBeat += beat / 4;

            foreach (Character c in CharacterPack)
                c.OnQuarterBeat();
        }

        public override void Update()
        {
            //Wait before we update Characters, that will mess this up

            while (Vitaru.ThreadsRunning()) { }

            Current = Clock.Current;
            LastElapsedTime = Clock.LastElapsedTime;

            base.Update();

            if (nextHalfBeat <= Clock.LastCurrent && nextHalfBeat != -1)
                OnHalfBeat();

            if (nextQuarterBeat <= Clock.LastCurrent && nextQuarterBeat != -1)
                OnQuarterBeat();

            //this check is for the editor for now
            if (ActivePlayer != null)
            {
                AudioManager.Context.Listener.Position =
                    new Vector3(ActivePlayer.Position.X / 2, 0, ActivePlayer.Position.Y / 2);

                if (ActivePlayer.Health != LastHealth)
                {
                    Layers.HealthChange.ClearTransforms();
                    Layers.HealthBar.ClearTransforms();

                    float y = PrionMath.Remap(ActivePlayer.Health, 0, ActivePlayer.HealthCapacity, 0, MaxBarSize);

                    if (ActivePlayer.Health < LastHealth)
                    {
                        Layers.HealthChange.Color = Color.Red;
                        Layers.HealthChange.ReSize(new Vector2(8, y), 200, Easings.InQuad);

                        Layers.HealthBar.Color = Color.Yellow;
                        Layers.HealthBar.ColorTo(Color.White, beat * 4, Easings.InCirc);
                        Layers.HealthBar.Height = y;

                        Layers.EnergyBar.Color = Color.Cyan;
                        Layers.EnergyBar.ColorTo(Color.White, beat * 2, Easings.InCirc);
                    }

                    if (ActivePlayer.Health > LastHealth)
                    {
                        Layers.HealthChange.Color = Color.LimeGreen;
                        Layers.HealthChange.Height = y;

                        Layers.HealthBar.ReSize(new Vector2(8, y), 200, Easings.InQuad);
                    }

                    LastHealth = ActivePlayer.Health;
                    Layers.CurrentHealthText.Text = $"{Math.Round(ActivePlayer.Health, 0)} HP";
                    Layers.CurrentHealthText.Y = MaxBarSize / 2 - y + 16;
                }

                if (ActivePlayer.Energy != LastEnergy)
                {
                    Layers.EnergyChange.ClearTransforms();
                    Layers.EnergyBar.ClearTransforms();

                    float y = PrionMath.Remap(ActivePlayer.Energy, 0, ActivePlayer.EnergyCapacity, 0, MaxBarSize);

                    if (ActivePlayer.Energy < LastEnergy)
                    {
                        Layers.EnergyChange.Color = Color.BlueViolet;
                        Layers.EnergyChange.ReSize(new Vector2(8, y), 200, Easings.InQuad);

                        Layers.EnergyBar.Height = y;
                    }

                    if (ActivePlayer.Energy > LastEnergy)
                    {
                        Layers.EnergyChange.Color = Color.Blue;
                        Layers.EnergyChange.Height = y;

                        Layers.EnergyBar.ReSize(new Vector2(8, y), 200, Easings.InQuad);
                    }

                    LastEnergy = ActivePlayer.Energy;
                    Layers.CurrentEnergyText.Text = $"{Math.Round(ActivePlayer.Energy, 0)} SP";
                    Layers.CurrentEnergyText.Y = MaxBarSize / 2 - y + 16;
                }
            }

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
                ParticleLayer.UpdateParticles(0, particle_cap, (float)Clock.LastElapsedTime);

            //should be safe to kill them from here
            while (deadEnemyQue.TryDequeue(out Enemy e))
            {
                Debugger.Assert(!e.Disposed, $"Disposed {nameof(Enemy)}s shouldn't be in the {nameof(deadEnemyQue)}!");
                LoadedEnemies.Remove(e, false);
                UnloadedEnemies.Add(e);
            }

            while (deadPlayerQue.TryDequeue(out Player p))
            {
                Debugger.Assert(!p.Disposed,
                    $"Disposed {nameof(Player)}s shouldn't be in the {nameof(deadPlayerQue)}!");
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

        public void SetPlayer(Player player)
        {
            ActivePlayer = player;

            LastHealth = ActivePlayer.Health;
            LastEnergy = ActivePlayer.Energy;

            Layers.MaxHealthText.Text = $"{ActivePlayer.HealthCapacity} HP";
            Layers.MaxEnergyText.Text = $"{ActivePlayer.EnergyCapacity} SP";

            //HealthBar.Color = player.PrimaryColor;
            //EnergyBar.Color = player.PrimaryColor;
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
    }
}
