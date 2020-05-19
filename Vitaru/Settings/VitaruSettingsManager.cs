using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prion.Core.IO;
using Prion.Core.IO.Configs;

namespace Vitaru.Settings
{
    public class VitaruSettingsManager : SettingsManager<VitaruSetting>
    {
        public VitaruSettingsManager(Storage storage, bool init = true) : base(storage, init)
        {
        }

        protected override void InitDefaults()
        {
            throw new NotImplementedException();
        }
    }

    public enum VitaruSetting
    {

    }
}
