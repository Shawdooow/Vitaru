// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.IO;
using Prion.Nucleus.IO.Configs;

namespace Vitaru.Settings
{
    public class VitaruSettingsManager : SettingsManager<VitaruSetting>
    {
        public VitaruSettingsManager(Storage storage, bool init = true) : base(storage, init)
        {
        }

        protected override void InitDefaults()
        {
            SetValue(VitaruSetting.Touch, false);
            SetValue(VitaruSetting.DebugHacks, false);
        }
    }

    public enum VitaruSetting
    {
        Touch,
        DebugHacks
    }
}