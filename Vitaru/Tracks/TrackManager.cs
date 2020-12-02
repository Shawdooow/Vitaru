// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria;
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

        public static void SetAudioDefaults()
        {
            TrackManager.CurrentTrack.Gain = 1f;
            TrackManager.CurrentTrack.Pitch = 1;
            TrackManager.CurrentTrack.Rolloff = 0.1f;
            TrackManager.CurrentTrack.StereoDistance = new Vector3(4, 0, 0);
            TrackManager.CurrentTrack.Position = new Vector3(0, 0, -4);
            AudioManager.CurrentContext.Listener.Position = Vector3.Zero;
        }

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

            Sample sample;
            string file = $"{level.Title}\\{level.Filename}";

            if (!Game.SampleStore.ObjectDictionary.ContainsKey(file))
            {
                sample = AudioManager.CurrentContext.ConvertSample(
                    Vitaru.LevelStorage.GetStream(file),
                    $"{level.Filename}");
                Game.SampleStore.ObjectDictionary[file] = sample;
            }
            else
                sample = Game.SampleStore.ObjectDictionary[file];

            CurrentTrack = new Track(level, seek, sample)
            {
                DrawClock = linked
            };
            OnTrackChange?.Invoke(CurrentTrack);

            SetAudioDefaults();

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