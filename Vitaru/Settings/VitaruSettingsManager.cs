// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics;
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
            SetValue(VitaruSetting.DebugHacks, false);

            SetValue(VitaruSetting.PlayerVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.EnemyVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.BulletVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.LaserVisuals, GraphicsOptions.Pretty);
            SetValue(VitaruSetting.Particles, true);
            SetValue(VitaruSetting.ParticleMultiplier, 1f);
        }
    }

    public enum VitaruSetting
    {
        Touch,
        DebugHacks,

        //Graphics
        PlayerVisuals,
        EnemyVisuals,
        BulletVisuals,
        LaserVisuals,
        Particles,
        ParticleMultiplier,
    }

    public enum GraphicsOptions
    {
        Classic,
        Pretty,
        HighPerformance,
        Experimental
    }
}