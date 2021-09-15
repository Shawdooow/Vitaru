// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

        public List<Setting> UserSettings = new();

        public PlayerStatus Status = PlayerStatus.JoiningMatch;

        public byte[] Serialize()
        {
            List<byte> data = new();

            byte[] username = Username.ToLengthAndBytes();
            byte[] userid = BitConverter.GetBytes(ID);
            byte[] color = Color.ToLengthAndBytes();

            //Settings
            List<byte> settings = new();
            foreach (Setting setting in UserSettings)
                settings.AddRange(setting.Serialize());

            //Length of setting bytes
            byte[] settingslength = BitConverter.GetBytes(settings.Count);

            byte[] status = BitConverter.GetBytes((ushort)Status);

            data.AddRange(username);
            data.AddRange(userid);
            data.AddRange(color);
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
        ///     Does NOT includes the 4 bytes of (int)size of this <see cref="VitaruUser" />
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            UserSettings = new List<Setting>();

            int offset = 0;

            //start with name
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size = BitConverter.ToInt32(length);

            byte[] name = data.SubArray(offset, size);
            offset += name.Length;

            Username = Encoding.ASCII.GetString(name);

            //then ID...
            byte[] id = data.SubArray(offset, 8);
            offset += id.Length;

            ID = BitConverter.ToInt64(id);

            //Color
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);

            byte[] color = data.SubArray(offset, size);
            offset += color.Length;

            Color = Encoding.ASCII.GetString(color);

            //Settings now, should be a bit easier...
            length = data.SubArray(offset, 4);
            offset += length.Length;

            //Settings length
            int settings = BitConverter.ToInt32(length);
            settings += offset;

            int i = offset;
            while (i < settings)
            {
                //get setting size
                length = data.SubArray(i, 4);
                i += length.Length;
                size = BitConverter.ToInt32(length);

                byte[] setting = data.SubArray(i, size);
                i += setting.Length;

                Setting s = new();
                s.DeSerialize(setting);
                UserSettings.Add(s);
            }

            offset = settings;

            //Status
            byte[] status = data.SubArray(offset, 2);
            offset += status.Length;

            Status = (PlayerStatus)BitConverter.ToUInt16(status);
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
        [Description("Playing")] Playing,
    }
}