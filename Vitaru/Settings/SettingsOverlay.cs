// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Mitochondria.Graphics.UserInterface;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.IO.Configs;
using Prion.Nucleus.Utilities;
using Vitaru.Settings.Options;

namespace Vitaru.Settings
{
    public class SettingsOverlay : HoverableLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(SettingsOverlay);

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
                new ToggleOption<PrionSetting>(Game.PrionSettings, PrionSetting.Fullscreen)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Position = Vector2.Zero,
                    Size = new Vector2(width - 8, 20),
                    Text =
                    {
                        Text = "Toggle Fullscreen"
                    }
                },

                new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.Touch)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Position = new Vector2(0, 20),
                    Size = new Vector2(width - 8, 20),
                    Text =
                    {
                        Text = "Toggle Touch Mode"
                    }
                },
                new ToggleOption<VitaruSetting>(Vitaru.VitaruSettings, VitaruSetting.DebugHacks)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Position = new Vector2(0, 40),
                    Size = new Vector2(width - 8, 20),
                    Text =
                    {
                        Text = "Toggle \"GOD-KING\" Hacks"
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

                    OnClick = () =>
                    {
                        if (toggle.Alpha > 0)
                            Toggle();
                    }
                }
            };
        }

        public override bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButtons.Left && !Hovered && toggle.Alpha < 1)
                Hide();
            return base.OnMouseDown(e);
        }

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
                toggle.FadeTo(0, 200, Easings.OutCubic);
                Shown = true;
            }
        }

        public void Hide()
        {
            if (Shown)
            {
                this.MoveTo(new Vector2(width, 0), 200, Easings.OutCubic);
                toggle.FadeTo(1, 200, Easings.OutCubic);
                Shown = false;
            }
        }
    }
}