// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using OpenTK.Input;
using Prion.Application.Groups.Packs;
using Prion.Game;
using Prion.Game.Graphics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;
using Vitaru.Players;

namespace Vitaru
{
    public class MainMenuRoot : Root
    {
        public MainMenuRoot()
        {
            Renderer.Window.Title = "Vitaru";

            
            DrawablePlayer drawablePlayer = new DrawablePlayer();
            Player player = new Player(drawablePlayer);

            Add(player.InputHandler);

            Add(new Pack<Player>
            {
                Child = player,
            });

            Add(new SpriteLayer
            {
                ParentScaling = Axes.Both,

                Children = new[]
                {
                    new Sprite(Game.TextureStore.GetTexture("vitaru spring 2018.png"))
                    {
                        ParentScaling = Axes.Both,
                        Scale = new Vector2(0.75f),
                    },
                    new Box
                    {
                        ParentScaling = Axes.Both,
                        Color = Color.Black,
                        Alpha = 0.5f,
                        Scale = new Vector2(5),
                    },
                    drawablePlayer,
                },
            });
        }
    }

    public class VitaruInputManager : ConfigedInputHandler<VitaruActions>
    {
        public override string Filename => "vitaruinput";

        protected override void OnInitialiseDefaults()
        {
            InputSettings.SetValue(VitaruActions.Up, Key.W);
            InputSettings.SetValue(VitaruActions.Down, Key.S);
            InputSettings.SetValue(VitaruActions.Left, Key.A);
            InputSettings.SetValue(VitaruActions.Right, Key.D);
            InputSettings.SetValue(VitaruActions.Slow, Key.ShiftLeft);
        }
    }

    public enum VitaruActions
    {
        Up,
        Down,
        Left,
        Right,
        Slow,
        Shoot,
    }
}