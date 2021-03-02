using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prion.Centrosome;
using Prion.Centrosome.Packets.Types;
using Vitaru.Server.Match;
using Vitaru.Server.Server;

namespace Vitaru.Server.Packets.Types
{
    public abstract class MatchInfoPacket : VariableLengthPacket
    {
        public MatchInfo MatchInfo;

        protected MatchInfoPacket(ushort header) : base(header)
        {
        }

        public override byte[] Serialize()
        {
            //Fist lets convert each field one at a time to byte arrays
            byte[] name = MatchInfo.Name.ToLengthAndBytes();
            byte[] id = Unsafe.As<uint, byte[]>(ref MatchInfo.MatchID);
            byte[] host = Unsafe.As<long, byte[]>(ref MatchInfo.Host);

            //Make sure we list how many users are in this lobby
            int ul = MatchInfo.Users.Count;
            byte[] userslength = Unsafe.As<int, byte[]>(ref ul);
            List<byte> users = new();

            foreach (VitaruUser user in MatchInfo.Users)
            {
                byte[] username = user.Username.ToLengthAndBytes();
                byte[] userid = Unsafe.As<long, byte[]>(ref user.ID);
                byte[] color = user.Color.ToLengthAndBytes();
                byte[] country = user.Country.ToLengthAndBytes();

                //Make sure we list how many settings there are
                int us = user.UserSettings.Count;
                byte[] usersettingslength = Unsafe.As<int, byte[]>(ref us);
                List<byte> usersettings = new();
                foreach (Setting setting in user.UserSettings)
                {
                    byte[] settingname = setting.Name.ToLengthAndBytes();
                    byte[] settingvalue = setting.Value.ToLengthAndBytes();
                    byte[] settingsync = Unsafe.As<Sync, byte[]>(ref setting.Sync);

                    usersettings.AddRange(settingname);
                    usersettings.AddRange(settingvalue);
                    usersettings.AddRange(settingsync);
                }

                byte[] status = Unsafe.As<PlayerStatus, byte[]>(ref user.Status);

                users.AddRange(username);
                users.AddRange(userid);
                users.AddRange(color);
                users.AddRange(country);
                users.AddRange(usersettingslength);
                users.AddRange(usersettings);
                users.AddRange(status);
            }

            //Make sure we list how many settings there are
            int ls = MatchInfo.Settings.Count;
            byte[] settingslength = Unsafe.As<int, byte[]>(ref ls);
            List<byte> settings = new();

            foreach (Setting setting in MatchInfo.Settings)
            {
                byte[] settingname = setting.Name.ToLengthAndBytes();
                byte[] settingvalue = setting.Value.ToLengthAndBytes();
                byte[] settingsync = Unsafe.As<Sync, byte[]>(ref setting.Sync);

                settings.AddRange(settingname);
                settings.AddRange(settingvalue);
                settings.AddRange(settingsync);
            }

            //now lets add all the data to the master list
            List<byte> data = new();

            data.AddRange(name);
            data.AddRange(id);
            data.AddRange(host);
            data.AddRange(userslength);
            data.AddRange(users);
            data.AddRange(settingslength);
            data.AddRange(settings);
            //TODO: data.AddRange(level);

            Length = (uint)data.Count;
            return data.ToArray();
        }

        public override void DeSerialize(byte[] data)
        {
            MatchInfo = new MatchInfo();

            //string name = Encoding.ASCII.GetString();
        }
    }
}
