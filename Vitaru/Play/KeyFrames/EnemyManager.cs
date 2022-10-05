using Prion.Nucleus.Debug;
using System;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Play.Characters.Enemies;

namespace Vitaru.Play.KeyFrames
{
    public class EnemyManager : KeyFrameEntityManager<Enemy>
    {
        public EnemyManager() 
        {
            FormatConverter converter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();

            try
            {
                if (LevelStore.CurrentLevel.EnemyData != null)
                    UnloadedEntities.AddRange(converter.StringToEnemies(LevelStore.CurrentLevel.EnemyData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Enemies, purging bad data...", LogType.IO);
                UnloadedEntities.Clear();
                LevelStore.CurrentLevel.EnemyData = string.Empty;
            }
        }
    }
}
