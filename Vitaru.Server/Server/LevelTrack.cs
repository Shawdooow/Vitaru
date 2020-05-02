using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prion.Application.Utilities.Interfaces;

namespace Vitaru.Server.Server
{
    [Serializable]
    public class LevelTrack
    {
        public string Name;

        public string Filename;

        public string Artist;

        public double BPM;

        public double Offset;
    }
}
