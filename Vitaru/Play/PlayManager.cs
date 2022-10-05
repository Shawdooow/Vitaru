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
using Vitaru.Play.KeyFrames;
using Vitaru.Play.Projectiles;
using Vitaru.Play.Teams;

namespace Vitaru.Play
{
    public class PlayManager : Pack<IUpdatable>
    {
        public static double Current { get; private set; } = double.MinValue;

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

        public PlayManager()
        {
        }

        public override void PreLoading()
        {
            base.PreLoading();

            Add(CharacterPack);
            Add(EnemyManager);
            Add(ProjectileManager);
        }
    }
}
