// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Golgi.Audio.Tracks;
using Prion.Nucleus.Timing;
using System.Runtime;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;
using Vitaru.Settings;

namespace Vitaru.Roots
{
    public class PlayRoot : MenuRoot
    {
        public override string Name => nameof(PlayRoot);

        protected override bool UseLevelBackground => true;

        protected override GCLatencyMode GCLatencyMode => GCLatencyMode.SustainedLowLatency;

        public PlayLayers PlayLayers { get; private set; }
        public PlayManager PlayManager { get; private set; }

        private Player player;

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

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Add(PlayManager);

            PlayManager.Add(player);
            PlayManager.SetPlayer(player);
        }

        public override void RenderingPreLoading()
        {
            base.RenderingPreLoading();

            PlayLayers = new PlayLayers();
            PlayManager = new PlayManager(PlayLayers);

            player = GamemodeStore.SelectedGamemode.SelectedCharacter != string.Empty
                ? GamemodeStore.GetPlayer(GamemodeStore.SelectedGamemode.SelectedCharacter, PlayManager)
                : new Yuie(PlayManager);

            player.SetDrawable(new DrawablePlayer(player, PlayLayers.CharacterLayer));

            Add(PlayLayers.Border);

            Add(PlayLayers.Layer2Ds);
            Add(PlayLayers.Layer3Ds);
        }

        public override void Update()
        {
            TrackManager.CurrentTrack.Clock.Update();
            base.Update();
        }

        public override void PreRender()
        {
            TrackManager.CurrentTrack.DrawClock.Update();
            base.PreRender();
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            player = null;
        }
    }
}