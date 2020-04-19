using System;
using System.Collections.Generic;
using System.Text;
using Vitaru.Characters.Enemies;
using Vitaru.Server.Server;

namespace Vitaru.Editor
{
    public class LevelData : Level
    {
        public List<Enemy> Enemies { get; set; }
    }
}
