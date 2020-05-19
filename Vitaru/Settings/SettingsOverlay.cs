﻿using System.Numerics;
using System.Drawing;
using OpenTK.Input;
using Prion.Core.IO.Configs;
using Prion.Core.Utilities;
using Prion.Game;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Graphics.UserInterface;
using Prion.Game.Input.Events;
using Vitaru.Settings.Options;

namespace Vitaru.Settings
{
    public class SettingsOverlay : HoverableLayer<IDrawable2D>
    {
        private const float width = 400;
        private const float height = 600;

        protected bool Shown;

        private Button toggle;

        public SettingsOverlay()
        {
            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Size = new Vector2(width, height);
            Position = new Vector2(width, 0);


            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    Size = Size
                }, 
                new ToggleOption<PrionSetting>(Game.PrionSettings, PrionSetting.RayTracing)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Size = new Vector2(width - 8, 20),
                    Text =
                    {
                        Text = "Toggle Raytracing"
                    }
                },

                toggle = new Button
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,

                    Size = new Vector2(100, 200),

                    Background = Game.TextureStore.GetTexture("square.png"),
                    Text = "Settings",
                    SpriteText =
                    {
                        TextScale = 0.25f
                    },
                    BackgroundSprite =
                    {
                        Color = Color.DarkSlateBlue
                    },

                    OnClick = Toggle
                }
            };
        }

        //public override bool OnMouseDown(MouseButtonEvent e)
        //{
        //    if (e.Button == MouseButton.Left && !Hovered)
        //        Hide();
        //    return base.OnMouseDown(e);
        //}

        public void Toggle()
        {
            if (Shown)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (!Shown)
            {
                this.MoveTo(Vector2.Zero, 200, Easings.OutCubic);
                Shown = true;
            }
        }

        public void Hide()
        {
            if (Shown)
            {
                this.MoveTo(new Vector2(width, 0), 200, Easings.OutCubic);
                Shown = false;
            }
        }
    }
}
