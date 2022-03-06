// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Vitaru.Editor.IO;
using Vitaru.Play.Characters.Enemies;
using Vitaru.Play.Projectiles;

namespace Vitaru.Gamemodes.Vitaru
{
    public class VitaruConverter : FormatConverter
    {
        public override List<Enemy> StringToEnemies(string level) => throw new NotImplementedException();

        public override string EnemiesToString(List<Enemy> enemies) => throw new NotImplementedException();

        public override List<Projectile> StringToProjectiles(string pattern) => throw new NotImplementedException();

        public override string ProjectilesToString(List<Projectile> projectiles) => throw new NotImplementedException();
    }
}