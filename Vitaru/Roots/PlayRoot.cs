// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Runtime;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Timing;
using Vitaru.Gamemodes;
using Vitaru.Play;
using Vitaru.Settings;

namespace Vitaru.Roots
{
    public class PlayRoot : MenuRoot
    {
        public override string Name => nameof(PlayRoot);

        protected override bool UseLevelBackground => true;

        protected override GCLatencyMode GCLatencyMode => GCLatencyMode.SustainedLowLatency;

        public readonly Layer2D<IDrawable2D> CharacterLayer = new()
        {
            Name = "Character Layer",
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize(),
        };

        public readonly GamefieldBorder Border;

        public PlayRoot()
        {
            //TrackManager.SetTrackDefaults();
            TrackManager.CurrentTrack.Pitch = Vitaru.VitaruSettings.GetFloat(VitaruSetting.Speed);
            TrackManager.SetPositionalDefaults();

            TrackManager.CurrentTrack.DrawClock = new SeekableClock();

            TrackManager.CurrentTrack.DrawClock.Start();
            TrackManager.CurrentTrack.DrawClock.Seek(TrackManager.CurrentTrack.Clock.Current);
            TrackManager.CurrentTrack.DrawClock.Rate = TrackManager.CurrentTrack.SeekableClock.Rate;
        }

        public override void PreRender()
        {
            TrackManager.CurrentTrack.DrawClock.Update();
            base.PreRender();
        }
    }
}