// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Game;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        public MainMenuRoot()
        {
            Player player = new Player();

            Add(player.InputHandler);

            Add(new SpriteLayer
            {
                ParentScaling = Axes.Both,

                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("vitaru spring 2018.png"))
                    {
                        ParentScaling = Axes.Both,
                    },
                    new Box
                    {
                        ParentScaling = Axes.Both,
                        Color = Color.Black,
                        Alpha = 0.5f,
                    },
                    player,
                },
            });
        }

        private class Player : Sprite, IHasInputKeys<VitaruAction>
        {
            public BindInputHandler<VitaruAction> InputHandler { get; set; }

            public Player() : base(Game.TextureStore.GetTexture("Sakuya Izayoi.png"))
            {
                ParentOrigin = Mounts.Center;
                InputHandler = new VitaruInputManager();
                Size = new Vector2(100);
                Color = Color.Blue;
            }

            public override void PreRender()
            {
                base.PreRender();
                Position = GetNewPlayerPosition(0.0025f);
            }

            public bool Pressed(VitaruAction t)
            {
                return true;
            }

            public bool Released(VitaruAction t)
            {
                return true;
            }

            protected virtual Vector2 GetNewPlayerPosition(double playerSpeed)
            {
                Vector2 playerPosition = Position;

                double yTranslationDistance = playerSpeed * Clock.ElapsedTime;
                double xTranslationDistance = playerSpeed * Clock.ElapsedTime;

                if (InputHandler.Actions[VitaruAction.Slow])
                {
                    xTranslationDistance /= 2d;
                    yTranslationDistance /= 2d;
                }

                if (InputHandler.Actions[VitaruAction.Up])
                    playerPosition.Y += (float) yTranslationDistance;
                if (InputHandler.Actions[VitaruAction.Left])
                    playerPosition.X -= (float) xTranslationDistance;
                if (InputHandler.Actions[VitaruAction.Down])
                    playerPosition.Y -= (float) yTranslationDistance;
                if (InputHandler.Actions[VitaruAction.Right])
                    playerPosition.X += (float) xTranslationDistance;

                //if (!VitaruPlayfield.BOUNDLESS)
                //{
                //    playerPosition = Vector2.ComponentMin(playerPosition, ChapterSet.PlayfieldBounds.Zw);
                //    playerPosition = Vector2.ComponentMax(playerPosition, ChapterSet.PlayfieldBounds.Xy);
                //}

                return playerPosition;
            }
        }

        private class VitaruInputManager : ConfigedInputHandler<VitaruAction>
        {
            public override string Filename => "vitaruinput";

            protected override void OnInitialiseDefaults()
            {
                InputSettings.SetValue(VitaruAction.Up, Key.W);
                InputSettings.SetValue(VitaruAction.Down, Key.S);
                InputSettings.SetValue(VitaruAction.Left, Key.A);
                InputSettings.SetValue(VitaruAction.Right, Key.D);
                InputSettings.SetValue(VitaruAction.Slow, Key.ShiftLeft);
            }
        }

        private enum VitaruAction
        {
            Up,
            Down,
            Left,
            Right,
            Slow,
            Shoot,
        }
    }
}