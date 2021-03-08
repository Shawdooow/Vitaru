// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prion.Centrosome;
using Prion.Nucleus.Utilities.Interfaces;
using Vitaru.Server.Levels;
using Vitaru.Server.Server;

namespace Vitaru.Server.Match
{
    public class MatchInfo : ISerializableToBytes
    {
        public string Name = "Welcome to Vitaru!";

        public uint ID;

        /// <summary>
        ///     Lets assume they will always be in <see cref="Users"/> and just save their ID for sanity
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
            byte[] id = Unsafe.As<uint, byte[]>(ref ID);
            byte[] host = Unsafe.As<long, byte[]>(ref Host);

            //Make sure we list how many users are in this lobby
            int ul = Users.Count;
            byte[] userslength = Unsafe.As<int, byte[]>(ref ul);
            List<byte> users = new();

            foreach (VitaruUser user in Users)
                users.AddRange(user.Serialize());

            //Make sure we list how many settings there are
            int ls = Settings.Count;
            byte[] settingslength = Unsafe.As<int, byte[]>(ref ls);
            List<byte> settings = new();

            foreach (Setting setting in Settings)
                settings.AddRange(setting.Serialize());

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

            return data.ToArray();
        }

        public void DeSerialize(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}