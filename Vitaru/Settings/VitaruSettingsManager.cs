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
            SetValue(VitaruSetting.DebugHacks, false);
        }
    }

    public enum VitaruSetting
    {
        DebugHacks,
    }
}
