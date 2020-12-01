// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Prion.Mitochondria.Audio;
using Prion.Mitochondria.Audio.Contexts;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Timing;
using Vitaru.Levels;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public static class TrackManager
    {
        public static Track CurrentTrack { get; private set; }

        public static Action<Track> OnTrackChange;

        public static bool Switching { get; set; }

        public static void SetTrack(LevelTrack level, SeekableClock clock = null)
        {
            Switching = true;

            SeekableClock seek = clock ?? CurrentTrack.Clock;
            SeekableClock linked = CurrentTrack?.DrawClock;

            seek.Stop();
            seek.Reset();

            CurrentTrack?.Pause();
            CurrentTrack?.Dispose();

            Logger.Log($"Setting Track \"{level.Title}\"");

            Sample sample = AudioManager.CurrentContext.ConvertSample(Vitaru.LevelStorage.GetStream($"{level.Title}\\{level.Filename}"),
                $"{level.Filename}");
            CurrentTrack = new Track(level, seek, sample)
            {
                DrawClock = linked
            };
            OnTrackChange?.Invoke(CurrentTrack);

            seek.Start();
            CurrentTrack.Play();
            Switching = false;
        }

        public static void NextTrack()
        {
            LevelTrack next = LevelStore.SetRandomLevel(CurrentTrack.Level);
            SetTrack(next, CurrentTrack.Clock);
        }

        public static void TryNextTrack()
        {
            if (CurrentTrack.CheckFinish())
                NextTrack();
        }

        public static void TryRepeatTrack()
        {
            CurrentTrack.TryRepeat();
        }
    }
}