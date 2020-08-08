﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio.OpenAL;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Stores;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.IO;
using Prion.Nucleus.Platform;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Mods;
using Vitaru.Roots;
using Vitaru.Roots.Tests;
using Vitaru.Settings;
using Vitaru.Themes;

namespace Vitaru
{
    public class Vitaru : Game
    {
        /// <summary>
        ///     Bool for easter egg Alki mode.
        ///     It has a 1/50 chance of being true on startup and can not be set manually
        /// </summary>
        public static bool ALKI { get; private set; }

        private static readonly Benchmark startup = new Benchmark("Startup");

        public static void Main(string[] args)
        {
            startup.Start();
            ALKI = PrionMath.RandomNumber(0, 50) == 5;
            if (ALKI)
            {
                Logger.SystemConsole("ALKI", ConsoleColor.Magenta);
                ThemeManager.Theme = new Alki();
            }
            else
            {
                bool somber = PrionMath.RandomNumber(0, 5) == 2;
                if (somber)
                    ThemeManager.Theme = new Somber();
            }

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            using (Vitaru vitaru = new Vitaru(args))
            {
                Root root;

                if (EXPERIMENTAL)
                    root = new MainMenuRoot(vitaru);
                else
                    root = new TestMenu(vitaru);

                vitaru.Start(root);
            }
        }

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static TextureStore LevelTextureStore { get; protected set; }

        private readonly AudioDevice device;

        private const string host =
#if true
            "VitaruDebug";
#else
            "Vitaru";
#endif

        protected Vitaru(string[] args) : base(host, args)
        {
#if !PUBLISH
            EXPERIMENTAL = true;
#endif

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

            while (FreeProcessors > 0 && EXPERIMENTAL)
                CreateDynamicTask();

            device = new AudioDevice();

            #region Shaders

            //sprite.vert is shared for both shaders
            string vert = new StreamReader(ShaderStorage.GetStream("sprite.vert")).ReadToEnd();

            //Sprite
            Shader sv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, vert);
            Shader sf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                new StreamReader(ShaderStorage.GetStream("sprite_shade.frag")).ReadToEnd());

            Renderer.SpriteProgram.Dispose();
            Renderer.SpriteProgram = Renderer.ShaderManager.GetShaderProgram(sv, sf);

            GLShaderProgram sprite = (GLShaderProgram) Renderer.SpriteProgram;

            sprite.SetActive();

            sprite.Locations["projection"] = GLShaderManager.GetLocation(sprite, "projection");
            sprite.Locations["model"] = GLShaderManager.GetLocation(sprite, "model");
            sprite.Locations["size"] = GLShaderManager.GetLocation(sprite, "size");
            sprite.Locations["spriteTexture"] = GLShaderManager.GetLocation(sprite, "spriteTexture");
            sprite.Locations["alpha"] = GLShaderManager.GetLocation(sprite, "alpha");
            sprite.Locations["spriteColor"] = GLShaderManager.GetLocation(sprite, "spriteColor");
            sprite.Locations["shade"] = GLShaderManager.GetLocation(sprite, "shade");
            sprite.Locations["intensity"] = GLShaderManager.GetLocation(sprite, "intensity");

            Renderer.ShaderManager.ActiveShaderProgram = sprite;

            Renderer.ShaderManager.UpdateInt("spriteTexture", 0);
            Renderer.ShaderManager.UpdateInt("shade", 0);
            Renderer.ShaderManager.UpdateInt("intensity", 1);

            Renderer.OnResize += value =>
            {
                sprite.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = sprite;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter(
                    Renderer.Width / -2f,
                    Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };

            //Circle
            Shader cv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, vert);
            Shader cf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                new StreamReader(ShaderStorage.GetStream("circle_shade.frag")).ReadToEnd());

            Renderer.CircularProgram.Dispose();
            Renderer.CircularProgram = Renderer.ShaderManager.GetShaderProgram(cv, cf);

            GLShaderProgram circle = (GLShaderProgram) Renderer.CircularProgram;

            circle.SetActive();

            circle.Locations["projection"] = GLShaderManager.GetLocation(circle, "projection");
            circle.Locations["model"] = GLShaderManager.GetLocation(circle, "model");
            circle.Locations["size"] = GLShaderManager.GetLocation(circle, "size");
            circle.Locations["spriteTexture"] = GLShaderManager.GetLocation(circle, "spriteTexture");
            circle.Locations["alpha"] = GLShaderManager.GetLocation(circle, "alpha");
            circle.Locations["spriteColor"] = GLShaderManager.GetLocation(circle, "spriteColor");
            circle.Locations["startAngle"] = GLShaderManager.GetLocation(circle, "startAngle");
            circle.Locations["endAngle"] = GLShaderManager.GetLocation(circle, "endAngle");
            circle.Locations["shade"] = GLShaderManager.GetLocation(circle, "shade");

            Renderer.ShaderManager.ActiveShaderProgram = circle;

            Renderer.ShaderManager.UpdateInt("spriteTexture", 0);
            Renderer.ShaderManager.UpdateFloat("startAngle", 0);
            Renderer.ShaderManager.UpdateFloat("endAngle", (float) Math.PI * 2);
            Renderer.ShaderManager.UpdateInt("shade", 0);

            Renderer.OnResize += value =>
            {
                circle.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = circle;
                Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter(
                    Renderer.Width / -2f,
                    Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
            };

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));

            #endregion
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

        protected override void StartupComplete()
        {
            //base.StartupComplete();
            startup.Record();
            Logger.Benchmark(startup);
        }

        public override void Dispose()
        {
            VitaruSettings.Dispose();
            base.Dispose();
        }
    }
}