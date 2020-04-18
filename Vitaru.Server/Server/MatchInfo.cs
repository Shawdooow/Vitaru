using System;
using System.Collections.Generic;

namespace Vitaru.Server.Server
{
    [Serializable]
    public class MatchInfo
    {
        public string Name = @"Welcome to Vitaru!";

        public uint MatchID;

        public List<VitaruUser> Users = new List<VitaruUser>();

        public List<Setting> Settings = new List<Setting>();

        public VitaruUser Host;

        public Level Level;
    }
}
