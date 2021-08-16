// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Debug;
using Vitaru.Editor.IO;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Projectiles;

namespace Vitaru.Gamemodes.Vitaru
{
    public class VitaruConverter : FormatConverter
    {
        public override List<Enemy> StringToEnemies(string level)
        {
            List<Enemy> enemies = new();

            string[] data = level.Split(";", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < data.Length; i++)
            {
                string[] eData = data[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                enemies.Add(new Enemy(Gamefield));
                enemies.Last().ParseString(eData, 0);
            }

            return enemies;
        }

        public override string EnemiesToString(List<Enemy> enemies)
        {
            string level = "";

            for (int i = 0; i < enemies.Count; i++)
            {
                string[] data = enemies[i].SerializeToStrings();
                for (int j = 0; j < data.Length; j++)
                {
                    level += $"{data[j]}";
                    if (j < data.Length)
                        level += ",";
                }

                level += ";";
            }

            return level;
        }

        public override List<Projectile> StringToProjectiles(string pattern)
        {
            List<Projectile> projectiles = new();

            string[] data = pattern.Split(";");

            for (int i = 0; i < data.Length; i++)
            {
                string[] pData = data[i].Split(',');
                switch (pData[0])
                {
                    default:
                        Logger.Error($"Projectile type {pData[0]} not supported!", LogType.IO);
                        break;
                    case "b":
                        Bullet b = new();
                        b.ParseString(pData, 1);
                        projectiles.Add(b);
                        break;
                    case "l":
                        Logger.Warning("Lasers not supported yet!", LogType.IO);
                        break;
                    case "s":
                        Logger.Warning("Seeking Bullets not supported yet!", LogType.IO);
                        break;
                }
            }

            return projectiles;
        }

        public override string ProjectilesToString(List<Projectile> projectiles)
        {
            string pattern = "";

            for (int i = 0; i < projectiles.Count; i++)
            {
                string[] data = projectiles[i].SerializeToStrings();
                for (int j = 0; j < data.Length; j++)
                {
                    pattern += $"{data[j]}";
                    if (j < data.Length)
                        pattern += ",";
                }

                pattern += ";";
            }

            return pattern;
        }
    }
}