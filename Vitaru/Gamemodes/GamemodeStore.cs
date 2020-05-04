// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Gamemodes.Characters.Players;

namespace Vitaru.Gamemodes
{
    public static class GamemodeStore
    {
        public static List<LoadedGamemode> LoadedGamemodes { get; private set; } = new List<LoadedGamemode>();

        public static void ReloadGamemodes()
        {
            List<Gamemode> loadedGamemodes = new List<Gamemode>
            {
                new Vitaru.Vitaru()
                //new Tau.Tau(),
            };

            /*
            TODO: 3rd Party Gamemodes
            Dictionary<Assembly, Type> loadedAssemblies = new Dictionary<Assembly, Type>();
            
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "Vitaru.Gamemodes.*.dll"))
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
                loadedAssemblies.Values.Select(g => (Gamemode) Activator.CreateInstance(g)).ToList();
            
            foreach (Gamemode g in instances)
                loadedGamemodes.Add(g);

            */

            foreach (Gamemode g in loadedGamemodes)
                LoadedGamemodes.Add(new LoadedGamemode(g));
        }

        /// <summary>
        ///     Trys to find a loaded gamemode with the given name
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
        ///     Trys to find a loaded chapter with the given title
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
        ///     Trys to find a loaded player with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Player GetPlayer(string name)
        {
            foreach (LoadedGamemode set in LoadedGamemodes)
            foreach (Player p in set.Players)
                if (p.Name == name)
                    return p;
            return null;
        }

        public class LoadedGamemode
        {
            public readonly Gamemode Gamemode;

            public string SelectedCharacter;

            public readonly List<Chapter> Chapters = new List<Chapter>();

            public readonly List<Player> Players = new List<Player>();

            public LoadedGamemode(Gamemode gamemode)
            {
                Gamemode = gamemode;

                foreach (Chapter v in gamemode.GetChapters())
                    Chapters.Add(v);

                foreach (Chapter c in Chapters)
                foreach (Player v in c.GetPlayers())
                {
                    bool add = true;
                    foreach (Player p in Players)
                        if (p.Name == v.Name)
                        {
                            add = false;
                            break;
                        }

                    if (add)
                        Players.Add(v);
                }
            }
        }
    }
}