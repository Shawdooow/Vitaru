// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Text;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;
using Vitaru.Server.Levels;
using Vitaru.Server.Server;

namespace Vitaru.Server.Match
{
    public class MatchInfo : ISerializableToBytes
    {
        public string Name = "Welcome to Vitaru!";

        /// <summary>
        ///     Assigned by Server for keeping track of matches more easily
        /// </summary>
        public uint ID;

        /// <summary>
        ///     Lets assume they will always be in <see cref="Users" /> and just save their ID for sanity
        /// </summary>
        public long Host;

        public List<VitaruUser> Users = new();

        public List<Setting> Settings = new();

        public Level Level;

        public byte[] Serialize()
        {
            List<byte> data = new();

            //Fist lets convert each field one at a time to byte arrays
            byte[] name = Name.ToLengthAndBytes();
            byte[] id = BitConverter.GetBytes(ID);
            byte[] host = BitConverter.GetBytes(Host);

            //Users
            List<byte> users = new();

            foreach (VitaruUser user in Users)
                users.AddRange(user.Serialize());

            //Length of user bytes
            byte[] userslength = BitConverter.GetBytes(users.Count);

            //Settings
            List<byte> settings = new();

            foreach (Setting setting in Settings)
                settings.AddRange(setting.Serialize());

            //Length of setting bytes
            byte[] settingslength = BitConverter.GetBytes(settings.Count);

            byte[] level = Level.Serialize();

            //now lets add all the data to the master list
            data.AddRange(name);
            data.AddRange(id);
            data.AddRange(host);
            data.AddRange(userslength);
            data.AddRange(users);
            data.AddRange(settingslength);
            data.AddRange(settings);
            data.AddRange(level);

            //last we stick the size in.
            //while it is technically possible to deduce the size of the data on the other side it is wildly impractical to implement
            byte[] size = BitConverter.GetBytes(data.Count);
            data.InsertRange(0, size);

            return data.ToArray();
        }


        /// <summary>
        /// Includes the 4 bytes of (int)size of this <see cref="MatchInfo"/>
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(byte[] data)
        {
            Users = new List<VitaruUser>();
            Settings = new List<Setting>();

            int offset = 4;

            //start with name
            byte[] length = data.SubArray(offset, 4);
            offset += length.Length;
            int size = BitConverter.ToInt32(length);

            byte[] name = data.SubArray(offset, size);
            offset += name.Length;

            Name = Encoding.ASCII.GetString(name);

            //next is ID
            byte[] id = data.SubArray(offset, 4);
            offset += id.Length;

            ID = BitConverter.ToUInt32(id);

            //then Host...
            byte[] host = data.SubArray(offset, 8);
            offset += host.Length;

            Host = BitConverter.ToInt64(host);

            //Users is gonna suck, please hold me...
            length = data.SubArray(offset, 4);
            offset += length.Length;

            //Users length
            int users = BitConverter.ToInt32(length);
            users += offset;

            int i = offset;
            while (i < users)
            {
                //get user size
                length = data.SubArray(i, 4);
                i += length.Length;
                size = BitConverter.ToInt32(length);

                byte[] user = data.SubArray(i, size);
                i += user.Length;

                VitaruUser v = new();
                v.DeSerialize(user);
                Users.Add(v);
            }

            offset = users;

            //Settings now, should be a bit easier...
            length = data.SubArray(offset, 4);
            offset += length.Length;

            //Settings length
            int settings = BitConverter.ToInt32(length);
            settings += offset;

            i = offset;
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
                Settings.Add(s);
            }

            offset = settings;

            //get level size
            length = data.SubArray(offset, 4);
            offset += length.Length;
            size = BitConverter.ToInt32(length);
            
            byte[] level = data.SubArray(offset, size);
            offset += level.Length;
            
            Level l = new();
            l.DeSerialize(level);
            Level = l;
        }
    }
}