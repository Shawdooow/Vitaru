// Copyright (c) 2018-2021 Shawn Bozek.
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

        public static void SetTrackDefaults()
        {
            CurrentTrack.Gain = 1f;
            CurrentTrack.Pitch = 1;
        }

        public static void SetPositionalDefaults()
        {
            CurrentTrack.Rolloff = 0.1f;
            CurrentTrack.StereoDistance = new Vector3(4, 0, 0);
            CurrentTrack.Position = new Vector3(0, 0, -4);
            AudioManager.Context.Listener.Position = Vector3.Zero;
            AudioManager.Context.Listener.Direction = new Vector3(0, 0, -1);
        }

        public static void SetTrack(LevelTrack level, SeekableClock clock = null)
        {
            Switching = true;

            SeekableClock seek = clock ?? CurrentTrack.SeekableClock;
            SeekableClock linked = CurrentTrack?.DrawClock;

            seek.Stop();
            seek.Reset();

            Track old = CurrentTrack;
            old?.Pause();

            Logger.Log($"Setting Track \"{level.Title}\"");

            Sample sample;
            string file = $"{level.Title}\\{level.Filename}";

            if (!Game.SampleStore.ObjectDictionary.ContainsKey(file))
            {
                sample = AudioManager.Context.ConvertSample(
                    Vitaru.LevelStorage.GetStream(file),
                    $"{level.Filename}");
                Game.SampleStore.ObjectDictionary[file] = sample;
            }
            else
                sample = Game.SampleStore.ObjectDictionary[file];

            seek.Reset();
            CurrentTrack = new Track(level, seek, sample)
            {
                DrawClock = linked
            };

            if (old != null)
            {
                CurrentTrack.Gain = old.Gain;
                CurrentTrack.Pitch = old.Pitch;
                CurrentTrack.Rolloff = old.Rolloff;

                CurrentTrack.StereoDistance = old.StereoDistance;
                CurrentTrack.Position = old.Position;

                old.Dispose();
            }

            OnTrackChange?.Invoke(CurrentTrack);

            seek.Start();
            CurrentTrack.Play();
            Switching = false;
        }

        public static void NextTrack()
        {
            LevelTrack next = LevelStore.SetRandomLevel(LevelStore.CurrentPack);
            SetTrack(next, CurrentTrack.SeekableClock);
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