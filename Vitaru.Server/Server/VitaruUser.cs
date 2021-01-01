// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vitaru.Server.Server
{
    /// <summary>
    ///     Vitaru User information
    ///     DOES NOT INCLUDE PASSWORD!!!
    /// </summary>
    [Serializable]
    public class VitaruUser
    {
        public string Username = "Alex";

        public long ID = -1;

        public string Colour = "#ffffff";

        public string Country;

        public List<Setting> UserSettings = new List<Setting>();

        public PlayerStatues Statues;
    }

    public enum PlayerStatues
    {
        [Description("Joining...")] JoiningMatch,
        [Description("Missing Gamemode")] MissingGamemode,
        [Description("Missing Level")] MissingLevel,

        [Description("Searching For Level...")]
        SearchingForLevel,
        [Description("Not Ready")] FoundMap,
        [Description("Ready")] Ready,
        [Description("Loading...")] Loading,
        [Description("Playing")] Playing
    }
}