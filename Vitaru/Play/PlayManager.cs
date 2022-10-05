using Prion.Nucleus.Debug;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Play.Characters;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;
using Vitaru.Play.Teams;

namespace Vitaru.Play
{
    public class PlayManager : Pack<IUpdatable>
    {
        public static double Current { get; private set; } = double.MinValue;

        protected readonly FormatConverter FormatConverter;

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

        public readonly Pack<Projectile> ProjectilePack = new()
        {
            Name = "Projectile Pack",
        };

        protected readonly List<Enemy> UnloadedEnemies = new List<Enemy>();
        protected readonly List<Projectile> UnloadedProjectiles = new List<Projectile>();

        public PlayManager()
        {
            FormatConverter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();

            //Enemies?
            try
            {
                if (LevelStore.CurrentLevel.EnemyData != null)
                    UnloadedEnemies.AddRange(FormatConverter.StringToEnemies(LevelStore.CurrentLevel.EnemyData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Enemies, purging bad data...", LogType.IO);
                UnloadedEnemies.Clear();
                LevelStore.CurrentLevel.EnemyData = string.Empty;
            }

            //Projectiles?
            try
            {
                if (LevelStore.CurrentLevel.ProjectileData != null)
                    UnloadedProjectiles.AddRange(FormatConverter.StringToProjectiles(LevelStore.CurrentLevel.ProjectileData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Projectiles, purging bad data...", LogType.IO);
                UnloadedProjectiles.Clear();
                LevelStore.CurrentLevel.ProjectileData = string.Empty;
            }
        }

        public override void PreLoading()
        {
            base.PreLoading();

            Add(CharacterPack);
            Add(ProjectilePack);
        }
    }
}
