// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using Prion.Golgi.Themes;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Contexts.GL46;
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
using Vitaru.Input;
using Vitaru.Levels;
using Vitaru.Mods;
using Vitaru.Roots;
using Vitaru.Server;
using Vitaru.Settings;

namespace Vitaru
{
    public class Vitaru : Game
    {
        public const string VERSION = VitaruServer.VERSION;

        /// <summary>
        ///     For Online Connections.
        ///     Often contains the same value more than once
        /// </summary>
        public static readonly string[] BACKWARDS_COMPATIBLE_VERSIONS =
        {
            VERSION,
            VitaruServer.VERSION,
        };

        /// <summary>
        ///     Bool for easter egg Alki mode.
        ///     It has a 1/100 chance of being 1 or greater on startup and can not be set manually
        /// </summary>
        public static byte ALKI { get; private set; }

        private static readonly Benchmark startup = new("Startup");

        private const string name = "Vitaru";

        /// <summary>
        ///     StartupObject EntryPoint
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            startup.Start();


            #region Startup


            //Do Launch args stuff first incase someone wants STATIC READONLYS!!!!
            List<string> launch = new(args);

#if !PUBLISH || PERSONAL
            if (!launch.Any(arg => arg.Contains("Features")))
                launch.Add($"Features={Features.Radioactive}");

            //if (!launch.Any(arg => arg.Contains("GContext")))
            //    launch.Add("GContext=GL41");
#endif

            VitaruLaunchArgs v = new()
            {
                Name = name,
            };
            VitaruLaunchArgs.ProccessArgs(launch.ToArray());


            #endregion


            //Easter Egg Time...
            ALKI = PrionMath.RandomNumber(0, 100) == 4 ? (byte)1 : (byte)0;

            if (ALKI == 1)
            {
                if (PrionMath.RandomNumber(0, 5) == 4)
                {
                    ALKI = 2;
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
                if (false) //somber)
                    ThemeManager.Theme = new Somber();
            }

            //OK Now buckle your fuckle, we are ready to go!
            Vitaru vitaru = new(v);
            vitaru.Start(new MainMenu(vitaru));
        }

        public static bool EnableTanks => FEATURES >= Features.Experimental;
        public static bool EnableMulti => FEATURES >= Features.Radioactive;

        public static VitaruSettingsManager VitaruSettings { get; private set; }

        public static PlayerBinds PlayerBinds { get; private set; }

        public static Storage LevelStorage { get; protected set; }

        public static TextureStore LevelTextureStore { get; protected set; }

        public static ShaderProgram BulletProgram { get; protected set; }

        protected Vitaru(VitaruLaunchArgs args) : base(args)
        {
            VitaruSettings = new VitaruSettingsManager(ApplicationDataStorage);
            PlayerBinds = new PlayerBinds();
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

            int threads = Settings.GetInt(NucleusSetting.DynamicThreadCountOverride);

            if (threads <= -1)
            {
                while (FreeProcessors > 0)
                    CreateDynamicTask();
            }
            else
            {
                while (DynamicThreads.Count < threads)
                    CreateDynamicTask();
            }


            #region Shaders


            if (Renderer.Context is OpenGL46)
            {
                //Post
                Shader pv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, "Post",
                    new StreamReader(ShaderStorage.GetStream("post.vert")).ReadToEnd());
                Shader pf = Renderer.ShaderManager.GetShader(ShaderType.Pixel, "Post",
                    new StreamReader(ShaderStorage.GetStream("post_shade.frag")).ReadToEnd());

                Renderer.PostProgram.Dispose();
                Renderer.PostProgram = Renderer.ShaderManager.GetShaderProgram(pv, pf);

                GLShaderProgram post = (GLShaderProgram)Renderer.PostProgram;
                post.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = post;

                post.Locations["shade"] = GLShaderManager.GetLocation(post, "shade");
                post.Locations["intensity"] = GLShaderManager.GetLocation(post, "intensity");

                Renderer.ShaderManager.UpdateInt("shade", 0);
                Renderer.ShaderManager.UpdateFloat("intensity", 1);

                //Bullet
                Shader bv = Renderer.ShaderManager.GetShader(ShaderType.Vertex, "Bullets",
                    new StreamReader(ShaderStorage.GetStream("bullet.vert")).ReadToEnd());
                Shader bf = Renderer.ShaderManager.GetShader(ShaderType.Pixel, "Bullets",
                    new StreamReader(ShaderStorage.GetStream("bullet.frag")).ReadToEnd());

                BulletProgram = Renderer.ShaderManager.GetShaderProgram(bv, bf);

                GLShaderProgram bp = (GLShaderProgram)BulletProgram;
                bp.SetActive();
                Renderer.ShaderManager.ActiveShaderProgram = bp;

                bp.Locations["projection"] = GLShaderManager.GetLocation(bp, "projection");
                bp.Locations["scale"] = GLShaderManager.GetLocation(bp, "scale");

                bp.Locations["circleTexture"] = GLShaderManager.GetLocation(bp, "circleTexture");
                bp.Locations["glowTexture"] = GLShaderManager.GetLocation(bp, "glowTexture");

                Renderer.ShaderManager.UpdateVector2("scale", Vector2.One);
            }

            Renderer.OnResize.Invoke(Renderer.RenderSize);


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

            if (NewCursor)
            {
                TextureStore.GetTexture("Cursor\\glow.png");
                TextureStore.GetTexture("Cursor\\ring.png");
            }

            startup.Record();
            Logger.Benchmark(startup);
        }

        public override void Dispose()
        {
            VitaruSettings.Dispose();
            PlayerBinds.Dispose();
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
        Blue,
    }
}