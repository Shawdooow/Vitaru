// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using System.Runtime;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Groups.Packs;
using Prion.Nucleus.Timing;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Play;
using Vitaru.Play.Characters;
using Vitaru.Play.Characters.Players;
using Vitaru.Settings;

namespace Vitaru.Roots
{
    public class PlayRoot : MenuRoot
    {
        public override string Name => nameof(PlayRoot);

        protected override bool UseLevelBackground => true;

        protected override GCLatencyMode GCLatencyMode => GCLatencyMode.SustainedLowLatency;

        public static double Current { get; private set; } = double.MinValue;

        public virtual Shades Shade { get; set; }

        public virtual float Intensity { get; set; } = 1;

        protected readonly FormatConverter FormatConverter;

        public readonly Pack<Character> CharacterPack = new()
        {
            Name = "Character Pack",
        };

        public readonly Layer2D<IDrawable2D> CharacterLayer = new()
        {
            Name = "Character Layer",
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize(),
        };

        public Player ActivePlayer { get; protected set; }

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

            Current = double.MinValue;

            Vector2 size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();

            Add(CharacterPack);

            FormatConverter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();

            try
            {
                //if (LevelStore.CurrentLevel.EnemyData != null)
                    //UnloadedEnemies.AddRange(FormatConverter.StringToEnemies(LevelStore.CurrentLevel.EnemyData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Enemies, purging bad data...", LogType.IO);
                //UnloadedEnemies.Clear();
                LevelStore.CurrentLevel.EnemyData = string.Empty;
            }
        }

        public override void PreRender()
        {
            TrackManager.CurrentTrack.DrawClock.Update();
            base.PreRender();
        }
    }
}