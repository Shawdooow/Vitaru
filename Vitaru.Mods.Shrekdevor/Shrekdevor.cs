// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Vitaru.Roots;
using static Prion.Nucleus.Debug.Console;

namespace Vitaru.Mods.Shrekdevor
{
    public class Shrekdevor : Mod
    {
        public override string Name => "The Great Shrekdevor: REDICED!";

        public override Button GetMenuButton() =>
            new()
            {
                Y = 60,
                Size = new Vector2(200, 100),

                Background = Game.TextureStore.GetTexture("square.png"),
                BackgroundSprite =
                {
                    Color = Color.Lime,
                },

                Text = "Shrekdevor",
            };

        public override Root GetRoot() => new ShrekRoot();

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            shrek();
        }

        public static void ShrekState(ulong s)
        {
            State = s;
            Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>(GetShrekText(State), Color.Lime));
        }

        public static string GetShrekText(ulong state)
        {
            switch (state)
            {
                default:
                    return string.Empty;
                case start:
                    return "Welcome to the Great Shrekdevor REDICED Edition!\n\n" +
                            "You are walking down a path, which direction do you travel? (left, right)";
            }
        }

        public class ShrekRoot : MenuRoot
        {
            public override string Name => nameof(ShrekRoot);

            protected override bool UseLevelBackground => false;

            protected override bool Parallax => true;

            protected ulong PreviousState;

            public override void RenderingLoadingComplete()
            {
                base.RenderingLoadingComplete();
                SetBackground(State);
            }

            public override void PreRender()
            {
                base.PreRender();

                if (State != PreviousState)
                {
                    PreviousState = State;
                    SetBackground(State);
                }
            }

            protected void SetBackground(ulong state)
            {
                if (!Vitaru.AssetStorage.Exists($"Textures/Shrekdevor/{state}")) return;

                string[] files = Vitaru.AssetStorage.GetFiles($"Textures/Shrekdevor/{state}");

                int rand = PrionMath.RandomNumber(0, files.Length);

                Background.Texture = Vitaru.TextureStore.GetTexture($"Shrekdevor/{state}/{files[rand]}");
            }
        }


        #region Shrekdevor


        internal static ulong State;

        private const ulong start = 1;

        private const ulong left = 1000;
        private const ulong right = 2000;

        private const ulong forward = 3000;
        private const ulong back = 4000;
        private const ulong up = 5000;
        private const ulong down = 6000;

        private const ulong die = 7000;


        private void shrek()
        {
            Prion.Nucleus.Debug.Console.Commands.AddRange(new[]
            {
                //Intro is second to avoid a bug
                new ConsoleCommand("Shrekdevor", true) { OnCommand = args => ShrekState(start) },



                //Left
                new ConsoleCommand("left")
                {
                    OnCommand = args =>
                    {
                        if (State != start) return;
                        ShrekState(left);
                        State = left;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You walk down the left path.\n\n" +
                            "", Color.Lime));
                    }
                },



                //Right
                new ConsoleCommand("right")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = right;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You travel down the right path.\n\n" +
                            "", Color.Lime));
                    }
                },



                //forward
                new ConsoleCommand("forward")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = forward;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("Not really caring to make a decision you forage straight forward into the forest.\n\n" +
                            "", Color.Lime));
                    }
                },



                //back
                new ConsoleCommand("back")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = back;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You begin traveling backwards but fucked up, you are traveling backwards in time.\n\n" +
                            "", Color.Lime));
                    }
                },



                //up
                new ConsoleCommand("up")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = up;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("Despite usually being bound to earth by gravity you manage to start floating straight up.\n\n" +
                            "", Color.Lime));
                    }
                },



                //down
                new ConsoleCommand("down")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = down;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You begin burrowing into the ground. " +
                            "A few miles down you turn around to find shrek is behind you and is helping you dig. " +
                            "You continue until you hit the mantle. " +
                            "Shrek advises you turn back, what shall you do? (back, cointinue)\n\n" +
                            "", Color.Lime));
                    }
                },

                new ConsoleCommand("back")
                {
                    OnCommand = args =>
                    {
                        if (State != down) return;
                        State = down + 100;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You turn back and go to the surface, " +
                            "only to find that you are no longer on earth but on a giant onion in space with many layers. " +
                            "Lord fuckwad sent space pirates to space assasinate you, how do you retaliate? (golden ticket, golden onion, railgun, microwave gun)\n\n" +
                            "", Color.Lime));
                    }
                },
                new ConsoleCommand("golden ticket")
                {
                    OnCommand = args =>
                    {
                        if (State != down) return;
                        State = down + 100 + 10;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You pull a golden ticket out of seemingly nowhere and flail it around. " +
                            "The polar express appears from behind a asteroid and runs over all the space pirates. It then stops to pick you up, then heads toward the north pole.\n\n" +
                            "" +
                            "Once you reach the north pole you realize that all the elfs have been enslaved by Adolf Hitler, " +
                            "and the nazis are going to board the train and search for more elfs. " +
                            "You and shrek hop off just outside of town and meet up with the rebels trying to kill Hitler, who are being lead by Santa. " +
                            "His weapons storehouse is unlike anything you've ever seen; tanks, planes, nukes. " +
                            "You name it he had it. " +
                            "His spys inform you all that now is a good time to strike, however Santa wants to perform some ritual before you go. " +
                            "Whats the plan? (ritual, nuke, sneaky, siege)\n\n" +
                            "", Color.Lime));
                    }
                },
                new ConsoleCommand("golden onion")
                {
                    OnCommand = args =>
                    {
                        if (State != down) return;
                        State = down + 100 + 10;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You eat the golden onion. " +
                            "You feel as though you can fart the end of the world. " +
                            "Your fart is so powerful it could be compared to a sun once it has reached the end of its life. " +
                            "Everything is destoyed in the wake of your nuclear fart, even you.\n\n" +
                            "", Color.Lime));
                    }
                },

                new ConsoleCommand("continue")
                {
                    OnCommand = args =>
                    {
                        if (State != down) return;
                        State = down + 200;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You tell shrek you must continue, he understands and follows you down. " +
                            "You continue through the mantle until you reach an opening. You have found Hell. " +
                            "Where do you head next? (lava, center)\n\n" +
                            "", Color.Lime));
                    }
                },



                //die
                new ConsoleCommand("die")
                {
                    OnCommand = args =>
                    {
                        if (State != 1) return;
                        State = die;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("You drop dead instantly. " +
                            "You awake to the sweet, stinging scent of onions a few hours later and find shrek has revived you. (kill, dream)\n\n" +
                            "", Color.Lime));
                    }
                },
            });
        }


        #endregion


    }
}