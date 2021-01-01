// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using Vitaru.Server.Server;
using Vitaru.Server.Track;

namespace Vitaru.Server.Match
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