﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vitaru.Server.Track;

namespace Vitaru.Levels
{
    public static class LevelStore
    {
        public static LevelPack CurrentPack { get; private set; }

        public static List<LevelPack> LoadedLevels { get; private set; } = new List<LevelPack>();

        public static void ReloadLevelsFromFolders()
        {
            LoadedLevels = new List<LevelPack>();

            string[] directories = Vitaru.LevelStorage.GetDirectories();

            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Vitaru.LevelStorage.GetFiles(directories[i]);

                LevelPack pack = new LevelPack
                {
                    Name = directories[i]
                };
                List<Level> levels = new List<Level>();

                for (int j = 0; j < files.Length; j++)
                {
                    string[] ext = files[j].Split('.');

                    Level level = new Level();
                    LevelTrack track = new LevelTrack();

                    if (ext.Last() == "vitaru")
                    {
                        using (StreamReader reader = new StreamReader(Vitaru.LevelStorage.GetStream($"{directories[i]}\\{files[j]}")))
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
                                    case "BPM":
                                        track.BPM = double.Parse(line[1]);
                                        continue;
                                    case "AudioOffset":
                                        track.Offset = double.Parse(line[1]);
                                        continue;
                                    case "PreviewTime":
                                        continue;
                                    case "Title":
                                        track.Name = line[1];
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
                                }
                            }
                        }

                        level.LevelTrack = track;
                        levels.Add(level);
                    }
                }

                pack.Levels = levels.ToArray();
                LoadedLevels.Add(pack);
            }

            CurrentPack = LoadedLevels[0];
        }

        public static void ReloadLevelsFromDatabase()
        {
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
                        Name = data[0]
                    };
                }
            }

            CurrentPack = LoadedLevels[0];
        }

        public static void ReCreateDatabase()
        {
            string[] directories = Vitaru.LevelStorage.GetDirectories();

            string data = string.Empty;
            for (int i = 0;
                i < directories.Length;
                i++)
            {
            }
        }

        public static void PopulateDefaults()
        {
        }
    }
}