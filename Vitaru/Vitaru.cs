// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Audio.OpenAL;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Stores;
using Prion.Nucleus;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.IO;
using Prion.Nucleus.IO.Configs;
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
        ///     It has a 1/100 chance of being true on startup and can not be set manually
        /// </summary>
        public static bool ALKI { get; private set; }

        public static bool DX12 { get; private set; }

        private static readonly Benchmark startup = new Benchmark("Startup");

        public static void Main(string[] args)
        {
            startup.Start();

            ALKI = PrionMath.RandomNumber(0, 100) == 5;

            if (ALKI)
            {
                Logger.SystemConsole("ALKI", ConsoleColor.Magenta);
                ThemeManager.Theme = new Alki();
            }
            else
            {
                bool somber = PrionMath.RandomNumber(0, 20) == 2;
                if (somber)
                    ThemeManager.Theme = new Somber();
            }

            using (Vitaru vitaru = new Vitaru(args))
            {
                if (FEATURES >= Features.Experimental)
                    vitaru.Start(new MainMenuRoot(vitaru));
                else
                    vitaru.Start(new TestMenu(vitaru));
            }
        }

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static TextureStore LevelTextureStore { get; protected set; }

        public static ShaderProgram BulletProgram { get; protected set; }

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
            FEATURES = Features.Upcoming;
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

            int dtco = PrionSettings.GetInt(PrionSetting.DynamicThreadCountOverride);

            if (dtco <= -1)
            {
                while (FreeProcessors > 0)
                    CreateDynamicTask();
            }
            else
            {
                while (DynamicThreads.Count < dtco)
                    CreateDynamicTask();
            }

            device = new AudioDevice();

            #region Shaders

            if (!DX12)
            {
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

                Renderer.ShaderManager.ActiveShaderProgram = sprite;
                Renderer.ShaderManager.SetSpriteLocations();

                sprite.Locations["shade"] = GLShaderManager.GetLocation(sprite, "shade");
                sprite.Locations["intensity"] = GLShaderManager.GetLocation(sprite, "intensity");

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
                //Shader cv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, vert);
                //Shader cf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                //    new StreamReader(ShaderStorage.GetStream("circle_shade.frag")).ReadToEnd());
                //
                //Renderer.CircularProgram.Dispose();
                //Renderer.CircularProgram = Renderer.ShaderManager.GetShaderProgram(cv, cf);
                //
                //GLShaderProgram circle = (GLShaderProgram) Renderer.CircularProgram;
                //
                //circle.SetActive();
                //
                //Renderer.ShaderManager.ActiveShaderProgram = circle;
                //Renderer.ShaderManager.SetCircleLocations();
                //
                //circle.Locations["shade"] = GLShaderManager.GetLocation(circle, "shade");
                //
                //Renderer.ShaderManager.UpdateInt("shade", 0);
                //
                //Renderer.OnResize += value =>
                //{
                //    circle.SetActive();
                //    Renderer.ShaderManager.ActiveShaderProgram = circle;
                //    Renderer.ShaderManager.UpdateMatrix4("projection", Matrix4x4.CreateOrthographicOffCenter(
                //        Renderer.Width / -2f,
                //        Renderer.Width / 2f, Renderer.Height / 2f, Renderer.Height / -2f, 1, -1));
                //};

                //TODO: Gradient

                Shader bv = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                    new StreamReader(ShaderStorage.GetStream("bullet.vert")).ReadToEnd());
                Shader bf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                    new StreamReader(ShaderStorage.GetStream("bullet.frag")).ReadToEnd());

                BulletProgram = Renderer.ShaderManager.GetShaderProgram(bv, bf);
                BulletProgram.SetActive();

                GLShaderProgram gl = (GLShaderProgram) BulletProgram;

                gl.Locations["projection"] = GLShaderManager.GetLocation(gl, "projection");

                gl.Locations["circleTexture"] = GLShaderManager.GetLocation(gl, "circleTexture");
                gl.Locations["glowTexture"] = GLShaderManager.GetLocation(gl, "glowTexture");

                Renderer.ShaderManager.ActiveShaderProgram = BulletProgram;
            }

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));

            #endregion
        }

        protected override GraphicsContext GetContext(string name)
        {
            switch (name)
            {
                default:
                    return base.GetContext("GL46");
                case "Legacy":
                case "GL41":
                    return base.GetContext("GL41");
                case "DX12":
                    DX12 = true;
                    return base.GetContext("DX12");
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