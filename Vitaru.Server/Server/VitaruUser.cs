// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prion.Centrosome;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Server.Server
{
    /// <summary>
    ///     Vitaru User information
    ///     DOES NOT INCLUDE PASSWORD!!!
    /// </summary>
    public class VitaruUser : ISerializableToBytes
    {
        public string Username = "User";

        public long ID = -1;

        public string Color = "#ffffff";

        public string Country;

        public List<Setting> UserSettings = new();

        public PlayerStatus Status;

        public byte[] Serialize()
        {
            List<byte> data = new();

            byte[] username = Username.ToLengthAndBytes();
            byte[] userid = Unsafe.As<long, byte[]>(ref ID);
            byte[] color = Color.ToLengthAndBytes();
            byte[] country = Country.ToLengthAndBytes();

            //Make sure we list how many settings there are
            int us = UserSettings.Count;
            byte[] usersettingslength = Unsafe.As<int, byte[]>(ref us);
            List<byte> usersettings = new();
            foreach (Setting setting in UserSettings)
                usersettings.AddRange(setting.Serialize());

            byte[] status = Unsafe.As<PlayerStatus, byte[]>(ref Status);

            data.AddRange(username);
            data.AddRange(userid);
            data.AddRange(color);
            data.AddRange(country);
            data.AddRange(usersettingslength);
            data.AddRange(usersettings);
            data.AddRange(status);

            return data.ToArray();
        }

        public void DeSerialize(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum PlayerStatus
    {
        [Description("Joining...")] JoiningMatch,
        [Description("Missing Gamemode")] MissingGamemode,
        [Description("Missing Level")] MissingLevel,
        [Description("Downloading Level")] DownloadingLevel,

        [Description("Searching For Level...")]
        SearchingForLevel,
        [Description("Not Ready")] FoundMap,
        [Description("Ready")] Ready,
        [Description("Loading...")] Loading,
        [Description("Playing")] Playing
    }
}