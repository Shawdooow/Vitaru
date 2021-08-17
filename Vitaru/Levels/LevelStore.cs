// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.Utilities;
using Prion.Ribosome.Audio;
using Vitaru.Server.Levels;

namespace Vitaru.Levels
{
    public static class LevelStore
    {
        public static bool UseRandom { get; private set; }

        public static Level CurrentLevel { get; private set; }

        public static event Action<Level> OnLevelChange;

        public static LevelPack CurrentPack { get; private set; }

        public static event Action<LevelPack> OnPackChange; 

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
                    TrackMetadata track = new();

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
                                    case "Filtering":
                                        track.Filtering = line[1] == "true" || line[1] == "1";
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

                        level.Metadata = track;
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
                            Metadata = new TrackMetadata
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

            SetLevelPack(LoadedLevels[0]);

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

            SetLevelPack(LoadedLevels[0]);

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

        public static void SetLevelPack(LevelPack p, int index = 0) => SetLevelPack(p, p.Levels[index]);

        public static void SetLevelPack(LevelPack p, Level level)
        {
            CurrentPack = p;
            OnPackChange?.Invoke(CurrentPack);
            SetLevel(level);
        }

        public static void SetLevel(int index)
        {
            if (index >= 0) 
                SetLevel(CurrentPack.Levels[index]);
            else 
                UseRandom = true;
        }

        public static void SetLevel(Level level)
        {
            Debugger.Assert(CurrentPack.Levels.Contains(level));
            CurrentLevel = level;
            OnLevelChange?.Invoke(CurrentLevel);
            UseRandom = false;
        }

        public static LevelPack SetRandomLevelPack(LevelPack last)
        {
            SetLevelPack(GetRandomLevelPack(last));
            SetLevel(GetRandomLevel(null));

            return CurrentPack;
        }

        public static LevelPack GetRandomLevelPack(LevelPack last)
        {
            int random = PrionMath.RandomNumber(0, LoadedLevels.Count);

            for (int i = 0; i < 10; i++)
            {
                bool skip = true;

                if (i < 5)
                {
                    for (int j = 0; j < LoadedLevels[random].Levels.Length; j++)
                        if (LoadedLevels[random].Levels[j].Metadata.Autoplay)
                            skip = false;
                }
                else
                    skip = false;

                if (last != null && LoadedLevels[random].Title == last.Title || skip)
                    random = PrionMath.RandomNumber(0, LoadedLevels.Count);
                else
                    break;
            }

            return LoadedLevels[random];
        }

        public static Level SetRandomLevel(Level last)
        {
            SetLevel(GetRandomLevel(last));

            return CurrentLevel;
        }

        public static Level GetRandomLevel(Level last)
        {
            LevelPack p = last == null ? CurrentPack : GetLevelPack(last);

            int random = PrionMath.RandomNumber(0, p.Levels.Length);
            int j = 0;

            for (int i = 0; i < 2; i++)
            {
                if (last == null || p.Levels[random].Name != last.Name)
                    while (j < LoadedLevels[random].Levels.Length)
                    {
                        if (LoadedLevels[random].Levels[j].Metadata.Autoplay)
                            break;
                        j++;
                    }

                PrionMath.RandomNumber(0, p.Levels.Length);
            }

            return p.Levels[j];
        }

        public static Level GetLevel(TrackMetadata data)
        {
            foreach (LevelPack pack in LoadedLevels)
            foreach (Level level in pack.Levels)
                if (level.Metadata == data)
                    return level;

            return null;
        }

        public static LevelPack GetLevelPack(Level level)
        {
            foreach (LevelPack pack in LoadedLevels)
                if (pack.Levels.Contains(level))
                    return pack;

            return null;
        }

        public static void SaveCurrentLevel()
        {
            string path = $"{CurrentPack.Title}\\{CurrentLevel.Name}.vitaru";

            if (!Vitaru.LevelStorage.Exists(path))
                Vitaru.LevelStorage.CreateFile(path);

            string header = $"Format={CurrentLevel.Format}{Environment.NewLine}" +
                            $"Audio={CurrentLevel.Metadata.Filename}{Environment.NewLine}" +
                            $"Image={CurrentLevel.Metadata.Image}{Environment.NewLine}" +
                            $"Filtering={CurrentLevel.Metadata.Filtering}{Environment.NewLine}" +
                            $"Autoplay={CurrentLevel.Metadata.Autoplay}{Environment.NewLine}" +
                            $"BPM={CurrentLevel.Metadata.BPM}{Environment.NewLine}" +
                            $"AudioOffset={CurrentLevel.Metadata.Offset}{Environment.NewLine}" +
                            $"PreviewTime={CurrentLevel.Metadata.PreviewTime}{Environment.NewLine}" +
                            $"Title={CurrentLevel.Metadata.Title}{Environment.NewLine}" +
                            $"Artist={CurrentLevel.Metadata.Artist}{Environment.NewLine}" +
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