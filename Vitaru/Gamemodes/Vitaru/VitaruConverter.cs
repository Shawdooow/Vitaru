// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Gamemodes.Vitaru
{
    public class VitaruConverter : FormatConverter
    {
        public override List<Enemy> StringToEnemies(string level)
        {
            List<Enemy> enemies = new List<Enemy>();

            string[] data = level.Split(";");

            for (int i = 0; i < data.Length; i++)
            {
            }

            return enemies;
        }

        public override string EnemiesToString(List<Enemy> enemies)
        {
            string level = "Version=0.1;\n";

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
                if (i < enemies.Count)
                    level += "\n";
            }

            return level;
        }

        public override List<Projectile> StringToProjectiles(string pattern)
        {
            List<Projectile> projectiles = new List<Projectile>();

            string[] data = pattern.Split(";");

            for (int i = 0; i < data.Length; i++)
            {
            }

            return projectiles;
        }

        public override string ProjectilesToString(List<Projectile> projectiles)
        {
            string pattern = "Version=0.1;\n";

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
                if (i < projectiles.Count)
                    pattern += "\n";
            }

            return pattern;
        }
    }
}