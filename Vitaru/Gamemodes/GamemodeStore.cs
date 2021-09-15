// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prion.Nucleus.Debug;
using Vitaru.Chapters;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes
{
    public static class GamemodeStore
    {
        public const string GAMEMODE_NAME = "Vitaru.Gamemodes.*.dll";

        public static LoadedGamemode SelectedGamemode { get; private set; }

        public static List<LoadedGamemode> LoadedGamemodes { get; private set; } = new();

        private static Dictionary<Assembly, Type> loadedAssemblies;

        public static void ReloadGamemodes()
        {
            List<Gamemode> loadedGamemodes = new()
            {
                new Vitaru.Vitaru(),
                //new Tanks.Tanks(),
                //new Tau.Tau(),
            };

            loadedAssemblies = new Dictionary<Assembly, Type>();

            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, GAMEMODE_NAME))
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                if (loadedAssemblies.Values.Any(t => t.Namespace == filename)) return;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    loadedAssemblies[assembly] =
                        assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Gamemode)));
                }
                catch (Exception)
                {
                    Logger.Error($"Error loading a Gamemode from a chapter file! [filename = {filename}]");
                }
            }

            List<Gamemode> instances =
                loadedAssemblies.Values.Select(g => (Gamemode)Activator.CreateInstance(g)).ToList();

            foreach (Gamemode g in instances)
                loadedGamemodes.Add(g);

            foreach (Gamemode g in loadedGamemodes)
                LoadedGamemodes.Add(new LoadedGamemode(g));

            SelectedGamemode = LoadedGamemodes[0];
        }

        /// <summary>
        ///     Try to find a loaded gamemode with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Gamemode GetGamemode(string name)
        {
            foreach (LoadedGamemode set in LoadedGamemodes)
                if (set.Gamemode.Name == name)
                    return set.Gamemode;

            return null;
        }

        /// <summary>
        ///     Try to find a loaded chapter with the given title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static Chapter GetChapter(string title)
        {
            foreach (LoadedGamemode set in LoadedGamemodes)
                foreach (Chapter chapter in set.Chapters)
                    if (chapter.Title == title)
                        return chapter;
            return null;
        }

        /// <summary>
        ///     Try to find a loaded player with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Player GetPlayer(string name, Gamefield gamefield = null)
        {
            string[] split = name.Split(':');
            foreach (LoadedGamemode set in LoadedGamemodes)
                if (set.Gamemode.Name == split[0])
                    foreach (Chapter c in set.Chapters)
                        if (c.Title == split[1])
                            foreach (Player p in c.GetPlayers(gamefield))
                                if (p.Name == split[2])
                                    return p;
            return null;
        }

        public class LoadedGamemode
        {
            public readonly Gamemode Gamemode;

            public string SelectedCharacter
            {
                get => selected;
                set
                {
                    selected = $"{Gamemode.Name}:{value}";
                    OnSelectedCharacterChange?.Invoke(selected);
                }
            }

            private string selected;

            public event Action<string> OnSelectedCharacterChange;

            public readonly List<Chapter> Chapters = new();

            public readonly List<KeyValuePair<Chapter, Player>> Players = new();

            public LoadedGamemode(Gamemode gamemode)
            {
                Gamemode = gamemode;

                foreach (Chapter v in gamemode.GetChapters())
                    Chapters.Add(v);

                foreach (Chapter c in Chapters)
                    foreach (Player v in c.GetPlayers())
                    {
                        bool add = true;
                        foreach (KeyValuePair<Chapter, Player> p in Players)
                            if (p.Value.Name == v.Name)
                            {
                                add = false;
                                break;
                            }

                        if (add)
                            Players.Add(new KeyValuePair<Chapter, Player>(c, v));
                    }
            }
        }
    }
}