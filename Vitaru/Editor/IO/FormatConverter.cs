// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Editor.IO
{
    public abstract class FormatConverter
    {
        public abstract List<Enemy> StringToEnemies(string level);

        public abstract string EnemiesToString(List<Enemy> enemies);

        public abstract List<Projectile> StringToProjectiles(string pattern);

        public abstract string ProjectilesToString(List<Projectile> projectiles);
    }
}