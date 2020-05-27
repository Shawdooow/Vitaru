// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime;
using Prion.Core.Debug;
using Prion.Core.IO;
using Prion.Core.Platform;
using Prion.Core.Threads;
using Prion.Core.Utilities;
using Prion.Game;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Contexts;
using Prion.Game.Graphics.Contexts.GL46.Shaders;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Stores;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Mods;
using Vitaru.Roots.Tests;
using Vitaru.Settings;

namespace Vitaru
{
    public class Vitaru : Game
    {
        /// <summary>
        ///     Bool for easter egg Alki mode.
        /// </summary>
        public static bool ALKI { get; private set; }

        public static void Main(string[] args)
        {
            ALKI = PrionMath.RandomNumber(0, 10) == 5;
            if (ALKI) Logger.ConsoleLog("ALKI", ConsoleColor.Magenta);

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            using (Vitaru vitaru = new Vitaru(args))
                vitaru.Start(new TestMenu(vitaru));
        }

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static TextureStore LevelTextureStore { get; protected set; }

        public static readonly List<DynamicThread> Threads = new List<DynamicThread>();

        private readonly AudioDevice device;

        private const string host =
#if true
            "VitaruDebug";
#else
            "Vitaru";
#endif

        protected Vitaru(string[] args) : base(host, args)
        {
            VitaruSettings = new VitaruSettingsManager(ApplicationDataStorage);
            bool levels = ApplicationDataStorage.Exists("Levels");
            LevelStorage = ApplicationDataStorage.GetStorage("Levels");
            LevelTextureStore = new TextureStore(LevelStorage);

            if (!levels)
            {
                foreach (string level in AssetStorage.GetDirectories("Levels"))
                {
                    string source = $"{AssetStorage.Path}{PlatformInfo.Split}Levels{PlatformInfo.Split}{level}";
                    string destination = $"{LevelStorage.Path}{PlatformInfo.Split}{level}";
                    Storage.CopyDirectory(source, destination);
                }
            }

            while (FreeProcessors > 0)
                Threads.Add(CreateDynamicTask());

            device = new AudioDevice();

            GLShaderProgram sprite = (GLShaderProgram) Renderer.SpriteProgram;

            sprite.SetActive();
            sprite.Locations["shade"] = GLShaderManager.GetLocation(sprite, "shade");
            Renderer.ShaderManager.ActiveShaderProgram = sprite;
            Renderer.ShaderManager.UpdateInt("shade", 0);

            GLShaderProgram circle = (GLShaderProgram) Renderer.CircularProgram;

            circle.SetActive();
            circle.Locations["shade"] = GLShaderManager.GetLocation(circle, "shade");
            Renderer.ShaderManager.ActiveShaderProgram = circle;
            Renderer.ShaderManager.UpdateInt("shade", 0);
        }

        protected override GraphicsContext GetContext(string name)
        {
            //We don't want to load DX12 or Vulkan yet because they don't work
            switch (name)
            {
                case "Legacy":
                case "GL41":
                    return base.GetContext("GL41");
                default:
                    return base.GetContext("GL46");
            }
        }

        public override void Start()
        {
            Renderer.Window.Title = ALKI ? "Alki" : "Vitaru";
            Renderer.Window.Icon =
                new Icon(AssetStorage.GetStream(ALKI ? "Textures\\alki.ico" : "Textures\\vitaru.ico"));

            GamemodeStore.ReloadGamemodes();

            Modloader.ReloadMods();

            foreach (Mod mod in Modloader.LoadedMods)
                mod.LoadingComplete();

            LevelStore.ReloadLevelsFromFolders();

            base.Start();
        }

        public override void Dispose()
        {
            VitaruSettings.Dispose();
            base.Dispose();
        }

        /// <summary>
        ///     Get menu Background <see cref="Texture" />
        /// </summary>
        /// <returns></returns>
        public static Texture GetBackground() =>
            TextureStore.GetTexture(ALKI
                ? "Backgrounds\\Vitaru Fall BG 1440.png"
                : "Backgrounds\\vitaru spring 2018.png");
    }
}