// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.IO;
using Prion.Nucleus.IO.Configs;

namespace Vitaru.Settings
{
    public class VitaruSettingsManager : SettingsManager<VitaruSetting>
    {
        protected override string Filename => "vitaru.ini";

        public VitaruSettingsManager(Storage storage) : base(storage)
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
            SetValue(VitaruSetting.ParticleCap, 32786);
            SetValue(VitaruSetting.ParticleMultiplier, 1f);

            SetValue(VitaruSetting.ComboFire, true);

            SetValue(VitaruSetting.Multithreading, false);
            SetValue(VitaruSetting.BulletCap, 512);

            SetValue(VitaruSetting.DebugHacks, false);
            SetValue(VitaruSetting.EnemyMultiplier, 1);
            SetValue(VitaruSetting.BulletMultiplier, 1);
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
        ParticleCap,
        ParticleMultiplier,

        ComboFire,

        Multithreading,
        BulletCap,

        //Debug
        DebugHacks,
        EnemyMultiplier,
        BulletMultiplier,
    }

    public enum GraphicsOptions
    {
        Classic,
        Pretty,
        Performance,
        Experimental
    }
}