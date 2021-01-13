// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.DX12;
using Prion.Mitochondria.Graphics.Contexts.GL46.Shaders;
using Prion.Mitochondria.Graphics.Shaders;
using Prion.Mitochondria.Graphics.Stores;
using Prion.Nucleus;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Debug.Benchmarking;
using Prion.Nucleus.IO;
using Prion.Nucleus.Platform;
using Prion.Nucleus.Settings;
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
        public const string VERSION = "0.11.2";

        /// <summary>
        ///     Bool for easter egg Alki mode.
        ///     It has a 1/100 chance of being true on startup and can not be set manually
        /// </summary>
        public static byte ALKI { get; private set; }

        private static readonly Benchmark startup = new("Startup");

        private const string host =
#if true
            "VitaruDebug";
#else
            "Vitaru";
#endif

        public static void Main(string[] args)
        {
            startup.Start();

            ALKI = PrionMath.RandomNumber(0, 100) == 4 ? 1 : 0;

            if (ALKI == 1)
            {
                if (PrionMath.RandomNumber(0, 5) == 4)
                {
                    ALKI++;
                    Logger.SystemConsole("ALL RHIZE", ConsoleColor.DarkMagenta);
                    ThemeManager.Theme = new Rhize();
                }
                else
                {
                    Logger.SystemConsole("ALKI", ConsoleColor.Magenta);
                    ThemeManager.Theme = new Alki();
                }
            }
            else
            {
                bool somber = PrionMath.RandomNumber(0, 20) == 2;
                if (somber)
                    ThemeManager.Theme = new Somber();
            }

            List<string> launch = new(args);

#if !PUBLISH
            if (!launch.Any(arg => arg.Contains("Features")))
                launch.Add($"Features={Features.Experimental}");
#endif

            VitaruLaunchArgs v = new()
            {
                Name = host
            };
            VitaruLaunchArgs.ProccessArgs(launch.ToArray());

            using (Vitaru vitaru = new(v))
            {
                if (FEATURES >= Features.Radioactive)
                    vitaru.Start(new MainMenu(vitaru));
                else
                    vitaru.Start(new TestMenu(vitaru));
            }
        }

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static TextureStore LevelTextureStore { get; protected set; }

        public static ShaderProgram BulletProgram { get; protected set; }

        protected Vitaru(VitaruLaunchArgs args) : base(args)
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

            int dtco = Settings.GetInt(NucleusSetting.DynamicThreadCountOverride);

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

            #region Shaders

#if !PUBLISH
            if (!(Renderer.Context is DirectX12))
#endif
            {
                //Post
                Shader pv = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                    new StreamReader(ShaderStorage.GetStream("post.vert")).ReadToEnd());
                Shader pf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                    new StreamReader(ShaderStorage.GetStream("post_shade.frag")).ReadToEnd());

                Renderer.PostProgram.Dispose();
                Renderer.PostProgram = Renderer.ShaderManager.GetShaderProgram(pv, pf);

                GLShaderProgram post = (GLShaderProgram) Renderer.PostProgram;

                post.SetActive();

                Renderer.ShaderManager.ActiveShaderProgram = post;

                post.Locations["shade"] = GLShaderManager.GetLocation(post, "shade");
                post.Locations["intensity"] = GLShaderManager.GetLocation(post, "intensity");

                Renderer.ShaderManager.UpdateInt("shade", 0);
                Renderer.ShaderManager.UpdateInt("intensity", 1);

                //Bullet
                Shader bv = Renderer.ShaderManager.GetShader(ShaderType.Vertex,
                    new StreamReader(ShaderStorage.GetStream("bullet.vert")).ReadToEnd());
                Shader bf = Renderer.ShaderManager.GetShader(ShaderType.Pixel,
                    new StreamReader(ShaderStorage.GetStream("bullet.frag")).ReadToEnd());

                BulletProgram = Renderer.ShaderManager.GetShaderProgram(bv, bf);
                BulletProgram.SetActive();

                GLShaderProgram gl = (GLShaderProgram) BulletProgram;

                gl.Locations["projection"] = GLShaderManager.GetLocation(gl, "projection");
                gl.Locations["scale"] = GLShaderManager.GetLocation(gl, "scale");

                gl.Locations["circleTexture"] = GLShaderManager.GetLocation(gl, "circleTexture");
                gl.Locations["glowTexture"] = GLShaderManager.GetLocation(gl, "glowTexture");

                Renderer.ShaderManager.ActiveShaderProgram = BulletProgram;

                Renderer.ShaderManager.UpdateVector2("scale", Vector2.One);
            }

            Renderer.OnResize.Invoke(new Vector2(Renderer.RenderWidth, Renderer.RenderHeight));

            #endregion
        }

        public override void Start()
        {
            Renderer.Window.Title = ALKI > 0 ? ALKI == 2 ? "Rhize" : "Alki" : "Vitaru";
            Renderer.Window.Icon =
                new Icon(AssetStorage.GetStream(ALKI > 0
                    ? ALKI == 2 ? "Textures\\rhize.ico" : "Textures\\alki.ico"
                    : "Textures\\vitaru.ico"));

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

    public enum Shades
    {
        Color = 0,
        None = 0,
        Gray,
        Red,
        Green,
        Blue
    }
}