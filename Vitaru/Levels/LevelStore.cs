// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Utilities;
using Vitaru.Server.Levels;

namespace Vitaru.Levels
{
    public static class LevelStore
    {
        public static Level CurrentLevel { get; private set; }

        public static LevelPack CurrentPack { get; private set; }

        public static List<LevelPack> LoadedLevels { get; private set; } = new();

        #region Versions

        public const string EXPERIMENTAL = VERSION_01;
        public const string STABLE = VERSION_01;

        public const string BLANK_LEVEL = "BLANK";

        public const string VERSION_01 = "preview5.2";

        //Migrate to separate Header + Content files to make loading large libraries faster
        public const string VERSION_02 = "0.13.0 maybe?";

        //Migrate to a binary format for ultimate speed!
        public const string VERSION_03 = "before 1.0.0?";

        #endregion

        //TODO: Try Catch the shit out of this, we don't want to crash if a level is fucked
        public static void ReloadLevelsFromFolders()
        {
            Benchmark b = new("Reload Levels From Folders", true);

            LoadedLevels = new List<LevelPack>();

            string[] directories = Vitaru.LevelStorage.GetDirectories();

            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Vitaru.LevelStorage.GetFiles(directories[i]);

                LevelPack pack = new()
                {
                    Title = directories[i]
                };
                List<Level> levels = new();

                for (int j = 0; j < files.Length; j++)
                {
                    string[] ext = files[j].Split('.');

                    Level level = new();
                    LevelTrack track = new();

                    if (ext.Last() == "vitaru")
                    {
                        using (StreamReader reader =
                            new(Vitaru.LevelStorage.GetStream($"{directories[i]}\\{files[j]}")))
                        {
                            string contents = reader.ReadToEnd();
                            string[] lines = contents.Split(new[] {Environment.NewLine},
                                StringSplitOptions.RemoveEmptyEntries);

                            for (int k = 0; k < lines.Length; k++)
                            {
                                string[] line = lines[k].Split('=');

                                switch (line[0])
                                {
                                    default:
                                        continue;
                                    case "Format":
                                        level.Format = line[1];
                                        continue;
                                    case "Audio":
                                        track.Filename = line[1];
                                        continue;
                                    case "Image":
                                        track.Image = line[1];
                                        continue;
                                    case "Autoplay":
                                        track.Autoplay = line[1] == "true" || line[1] == "1";
                                        continue;
                                    case "BPM":
                                        track.BPM = double.Parse(line[1]);
                                        continue;
                                    case "AudioOffset":
                                        track.Offset = double.Parse(line[1]);
                                        continue;
                                    case "PreviewTime":
                                        track.PreviewTime = double.Parse(line[1]);
                                        continue;
                                    case "Title":
                                        track.Title = line[1];
                                        continue;
                                    case "Artist":
                                        track.Artist = line[1];
                                        continue;
                                    case "Creator":
                                        level.Creator = line[1];
                                        continue;
                                    case "Name":
                                        level.Name = line[1];
                                        continue;
                                    case "EnemyData":
                                        level.EnemyData = line[1];
                                        continue;
                                }
                            }
                        }

                        level.LevelTrack = track;
                        levels.Add(level);
                    }
                }

                if (!levels.Any())
                {
                    string audio = string.Empty;
                    string bg = string.Empty;

                    for (int j = 0; j < files.Length; j++)
                    {
                        string[] ext = files[j].Split('.');

                        if (ext.Last() == "mp3" || ext.Last() == "wav")
                            audio = files[j];
                        else if (ext.Last() == "png" || ext.Last() == "jpg" || ext.Last() == "jpeg")
                            bg = files[j];
                    }

                    if (audio != string.Empty)
                        levels.Add(new Level
                        {
                            Format = BLANK_LEVEL,
                            LevelTrack = new LevelTrack
                            {
                                Title = pack.Title,
                                Filename = audio,
                                Image = bg,
                                BPM = 120
                            }
                        });
                }

                pack.Levels = levels.ToArray();
                LoadedLevels.Add(pack);
            }

            CurrentPack = LoadedLevels[0];
            CurrentLevel = CurrentPack.Levels[0];

            b.Record();
            Logger.Benchmark(b);
        }

        //TODO: Try Catch this as a whole, if its broken just regen the whole thing
        public static void ReloadLevelsFromDatabase()
        {
            Benchmark b = new("Reload Levels From Database", true);

            LoadedLevels = new List<LevelPack>();
            using (StreamReader reader = new(Vitaru.LevelStorage.GetStream("database.vib")))
            {
                string contents = reader.ReadToEnd();
                string[] lines = contents.Split(';');
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(':');

                    LevelPack pack = new()
                    {
                        Title = data[0]
                    };
                }
            }

            CurrentPack = LoadedLevels[0];
            CurrentLevel = CurrentPack.Levels[0];

            b.Record();
            Logger.Benchmark(b);
        }

        public static void ReCreateDatabase()
        {
            Benchmark b = new("ReCreate Database", true);

            string[] directories = Vitaru.LevelStorage.GetDirectories();

            string data = string.Empty;
            for (int i = 0; i < directories.Length; i++)
            {
            }

            b.Record();
            Logger.Benchmark(b);
        }

        public static void PopulateDefaults()
        {
        }

        public static void SetLevel(LevelPack p)
        {
            CurrentPack = p;
            CurrentLevel = CurrentPack.Levels[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="last"></param>
        /// <returns></returns>
        public static LevelTrack SetRandomLevel(LevelPack last)
        {
            SetLevel(GetRandomLevel(last));

            return CurrentPack.Levels[0].LevelTrack;
        }

        public static LevelPack GetRandomLevel(LevelPack last)
        {
            int random = PrionMath.RandomNumber(0, LoadedLevels.Count);

            for (int i = 0; i < 10; i++)
            {
                if (last != null && LoadedLevels[random].Levels[0].LevelTrack.Title == last.Title ||
                    !LoadedLevels[random].Levels[0].LevelTrack.Autoplay)
                    random = PrionMath.RandomNumber(0, LoadedLevels.Count);
                else
                    break;
            }

            SetLevel(LoadedLevels[random]);

            return CurrentPack;
        }

        public static LevelPack GetLevelPack(LevelTrack track)
        {
            foreach (LevelPack pack in LoadedLevels)
            foreach (Level level in pack.Levels)
                if (level.LevelTrack == track)
                    return pack;

            return null;
        }

        public static void SaveCurrentLevel()
        {
            string path = $"{CurrentPack.Title}\\{CurrentLevel.Name}.vitaru";

            if (!Vitaru.LevelStorage.Exists(path))
                Vitaru.LevelStorage.CreateFile(path);

            string header = $"Format={CurrentLevel.Format}{Environment.NewLine}" +
                            $"Audio={CurrentLevel.LevelTrack.Filename}{Environment.NewLine}" +
                            $"Image={CurrentLevel.LevelTrack.Image}{Environment.NewLine}" +
                            $"BPM={CurrentLevel.LevelTrack.BPM}{Environment.NewLine}" +
                            $"AudioOffset={CurrentLevel.LevelTrack.Offset}{Environment.NewLine}" +
                            $"PreviewTime={CurrentLevel.LevelTrack.PreviewTime}{Environment.NewLine}" +
                            $"Title={CurrentLevel.LevelTrack.Title}{Environment.NewLine}" +
                            $"Artist={CurrentLevel.LevelTrack.Artist}{Environment.NewLine}" +
                            $"Creator={CurrentLevel.Creator}{Environment.NewLine}" +
                            $"Name={CurrentLevel.Name}{Environment.NewLine}" +
                            $"EnemyData={CurrentLevel.EnemyData}";

            using (StreamWriter writer =
                new(Vitaru.LevelStorage.GetStream(path, FileAccess.Write, FileMode.Truncate)))
            {
                Logger.Log($"Saving Current Level: {path}...", LogType.IO);
                writer.Write(header);
                Logger.Log("Current Level Saved!", LogType.IO);
            }
        }
    }
}