// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Utilities;
using Vitaru.Server.Track;

namespace Vitaru.Levels
{
    public static class LevelStore
    {
        public static Level CurrentLevel { get; private set; }

        public static LevelPack CurrentPack { get; private set; }

        public static List<LevelPack> LoadedLevels { get; private set; } = new List<LevelPack>();

        public const string BLANK_LEVEL = "BLANK";
        public const string VERSION_ONE = "preview5.2";

        //TODO: Try Catch the shit out of this, we don't want to crash if a level is fucked
        public static void ReloadLevelsFromFolders()
        {
            Benchmark b = new Benchmark("Reload Levels From Folders", true);

            LoadedLevels = new List<LevelPack>();

            string[] directories = Vitaru.LevelStorage.GetDirectories();

            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Vitaru.LevelStorage.GetFiles(directories[i]);

                LevelPack pack = new LevelPack
                {
                    Title = directories[i]
                };
                List<Level> levels = new List<Level>();

                for (int j = 0; j < files.Length; j++)
                {
                    string[] ext = files[j].Split('.');

                    Level level = new Level();
                    LevelTrack track = new LevelTrack();

                    if (ext.Last() == "vitaru")
                    {
                        using (StreamReader reader =
                            new StreamReader(Vitaru.LevelStorage.GetStream($"{directories[i]}\\{files[j]}")))
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
                                    case "BPM":
                                        track.BPM = double.Parse(line[1]);
                                        continue;
                                    case "AudioOffset":
                                        track.Offset = double.Parse(line[1]);
                                        continue;
                                    //case "PreviewStartTime":
                                    //track.PreviewStartTime = double.Parse(line[1]);
                                    //continue;
                                    case "Title":
                                        track.Title = line[1];
                                        continue;
                                    case "Artist":
                                        track.Artist = line[1];
                                        continue;
                                    case "Creator":
                                        level.LevelCreator = line[1];
                                        continue;
                                    case "Name":
                                        level.LevelName = line[1];
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
            Benchmark b = new Benchmark("Reload Levels From Database", true);

            LoadedLevels = new List<LevelPack>();
            using (StreamReader reader = new StreamReader(Vitaru.LevelStorage.GetStream("database.vib")))
            {
                string contents = reader.ReadToEnd();
                string[] lines = contents.Split(';');
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(':');

                    LevelPack pack = new LevelPack
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
            Benchmark b = new Benchmark("ReCreate Database", true);

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
        public static LevelTrack SetRandomLevel(LevelTrack last)
        {
            int random = PrionMath.RandomNumber(0, LoadedLevels.Count);

            for (int i = 0; i < 10; i++)
            {
                if (LoadedLevels[random].Levels[0].LevelTrack.Title == last.Title)
                    random = PrionMath.RandomNumber(0, LoadedLevels.Count);
                else
                    break;
            }

            SetLevel(LoadedLevels[random]);

            return CurrentPack.Levels[0].LevelTrack;
        }

        public static void SaveCurrentLevel()
        {
            //string path = $"{CurrentPack.Title}\\{CurrentLevel.LevelDifficulty}.vitaru";
            //
            //if (!Vitaru.LevelStorage.Exists(path))
            //    Vitaru.LevelStorage.CreateFile(path);
            //
            //using (StreamWriter writer =
            //    new StreamWriter(Vitaru.LevelStorage.GetStream(path)))
            //{
            //
            //}
        }
    }
}