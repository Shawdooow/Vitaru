using Prion.Nucleus.Debug;
using System;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Play.Projectiles;

namespace Vitaru.Play.KeyFrames
{
    public class ProjectileManager : KeyFrameEntityManager<Projectile>
    {
        public ProjectileManager() 
        {
            FormatConverter converter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();

            try
            {
                if (LevelStore.CurrentLevel.ProjectileData != null)
                    UnloadedEntities.AddRange(converter.StringToProjectiles(LevelStore.CurrentLevel.ProjectileData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Projectiles, purging bad data...", LogType.IO);
                UnloadedEntities.Clear();
                LevelStore.CurrentLevel.ProjectileData = string.Empty;
            }
        }
    }
}
