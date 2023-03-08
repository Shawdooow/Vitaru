// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

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
