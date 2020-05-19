// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime;
using Prion.Core.Debug;
using Prion.Core.IO;
using Prion.Core.Threads;
using Prion.Core.Utilities;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Contexts;
using Prion.Game.Graphics.Sprites;
using Vitaru.Gamemodes;
using Vitaru.Levels;
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
                vitaru.Start(new TestMenu());
        }

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static readonly List<DynamicThread> Threads = new List<DynamicThread>();

        protected Vitaru(string[] args) : base("vitaru", args)
        {
            VitaruSettings = new VitaruSettingsManager(ApplicationDataStorage);
            LevelStorage = ApplicationDataStorage.GetStorage("Levels");

            LevelStore.ReloadLevelsFromFolders();

            while (FreeProcessors > 0)
                Threads.Add(CreateDynamicTask());
        }

        protected override GraphicsContext GetContext(string name) => base.GetContext("GL46");

        public override void Start()
        {
            Renderer.Window.Title = ALKI ? "Alki" : "Vitaru";
            Renderer.Window.Icon =
                new Icon(AssetStorage.GetStream(ALKI ? "Textures\\alki.ico" : "Textures\\vitaru.ico"));

            GamemodeStore.ReloadGamemodes();

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