// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Prion.Centrosome;
using Prion.Nucleus.Utilities;
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

        public PlayerStatus Status = PlayerStatus.JoiningMatch;

        public byte[] Serialize()
        {
            List<byte> data = new();

            byte[] username = Username.ToLengthAndBytes();
            byte[] userid = BitConverter.GetBytes(ID);
            byte[] color = Color.ToLengthAndBytes();
            byte[] country = Country.ToLengthAndBytes();

            //Make sure we list how many settings there are
            int us = UserSettings.Count;
            byte[] settingslength = BitConverter.GetBytes(us);
            List<byte> settings = new();
            foreach (Setting setting in UserSettings)
                settings.AddRange(setting.Serialize());

            byte[] status = BitConverter.GetBytes((ushort) Status);

            data.AddRange(username);
            data.AddRange(userid);
            data.AddRange(color);
            data.AddRange(country);
            data.AddRange(settingslength);
            data.AddRange(settings);
            data.AddRange(status);

            //last we stick the size in.
            //while it is technically possible to deduce the size of the data on the other side it is wildly impractical to implement
            byte[] size = BitConverter.GetBytes(data.Count);
            data.InsertRange(0, size);

            return data.ToArray();
        }

        /// <summary>
        /// Does NOT includes the 4 bytes of (int)size of this <see cref="VitaruUser"/>
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            int offset = 0;

            //start with name
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size = BitConverter.ToInt32(length);

            byte[] name = data.SubArray(offset, size);
            offset += name.Length;

            Username = BitConverter.ToString(name);
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