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
using static System.Windows.Forms.AxHost;

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

            Commands.Add(new ConsoleCommand("reset") 
            { 
                OnCommand = args =>
                {
                    ShrekState.State = 0;
                    Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>("Shrekdevor State Reset!", Color.Lime));
                }
            });

            for (int i = 0; i < ShrekState.States.Length; i++)
            {
                ShrekState state = ShrekState.States[i];

                Commands.Add(new ConsoleCommand(state.InputText)
                {
                    OnCommand = args =>
                    {
                        if (ShrekState.State != state.InputState) return;

                        ShrekState.State = state.OutputState;
                        Renderer.Console.MessageQueue.Enqueue(new KeyValuePair<string, Color>(state.OutputText, Color.Lime));
                    }
                });
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
                SetBackground(ShrekState.State);
            }

            public override void PreRender()
            {
                base.PreRender();

                if (ShrekState.State != PreviousState)
                {
                    PreviousState = ShrekState.State;
                    SetBackground(ShrekState.State);
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
    }
}