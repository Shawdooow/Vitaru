// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.IO;
using Prion.Nucleus.IO.Configs;

namespace Vitaru.Settings
{
    public class VitaruSettingsManager : SettingsManager<VitaruSetting>
    {
        protected override string Filename => "vitaru.ini";

        public VitaruSettingsManager(Storage storage, bool init = true) : base(storage, init)
        {
        }

        protected override void InitDefaults()
        {
            SetValue(VitaruSetting.Touch, false);

            SetValue(VitaruSetting.PlayerVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.EnemyVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.BulletVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.LaserVisuals, GraphicsOptions.Pretty);

            SetValue(VitaruSetting.Particles, true);
            SetValue(VitaruSetting.ParticleMultiplier, 1f);
            SetValue(VitaruSetting.ComboFire, true);

            SetValue(VitaruSetting.ThreadBullets, false);

            SetValue(VitaruSetting.DebugHacks, false);
        }
    }

    public enum VitaruSetting
    {
        //Controls
        Touch,

        //Graphics
        PlayerVisuals,
        EnemyVisuals,
        BulletVisuals,
        LaserVisuals,

        Particles,
        ParticleMultiplier,
        ComboFire,

        //Performance
        ThreadBullets,

        //Debug
        DebugHacks,
    }

    public enum GraphicsOptions
    {
        Classic,
        Pretty,
        Performance,
        Experimental
    }
}