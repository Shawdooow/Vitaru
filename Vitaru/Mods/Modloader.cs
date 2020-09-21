// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prion.Nucleus.Debug;
using Vitaru.Gamemodes;
using Vitaru.Mods.Included;

namespace Vitaru.Mods
{
    public static class Modloader
    {
        public const string MOD_NAME = "Vitaru.Mods.*.dll";

        public static List<Mod> LoadedMods = new List<Mod>();

        private static Dictionary<Assembly, Type> loadedAssemblies;

        public static void ReloadMods()
        {
            LoadedMods = new List<Mod>
            {
                new Included.Prion(),
                new Mixer(),
                new Tanks(),
                new Nvidia()
            };

            loadedAssemblies = new Dictionary<Assembly, Type>();

            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, MOD_NAME))
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                if (loadedAssemblies.Values.Any(t => t.Namespace == filename)) return;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    loadedAssemblies[assembly] =
                        assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Mod)));
                }
                catch (Exception)
                {
                    Logger.Error("Error loading a mod file! [filename = " + filename + "]");
                }
            }

            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, GamemodeStore.GAMEMODE_NAME))
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                if (loadedAssemblies.Values.Any(t => t.Namespace == filename)) return;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    loadedAssemblies[assembly] =
                        assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Mod)));
                }
                catch (Exception)
                {
                    Logger.Log("Error loading a mod from a gamemode (it probably didn't have one [filename = " +
                               filename + "])");
                }
            }

            List<Mod> instances = loadedAssemblies.Values.Select(g => (Mod) Activator.CreateInstance(g)).ToList();

            //add any other mods
            foreach (Mod s in instances)
                LoadedMods.Add(s);
        }
    }
}