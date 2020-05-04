// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Input.Events;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Characters.Enemies;

namespace Vitaru.Editor.UI
{
    public class Toolbox : Layer2D<IDrawable2D>
    {
        public override string Name => "Toolbox";

        private const float width = 140;
        private const float height = 400;

        private readonly Layer2D<ToolboxItem> items;

        public Toolbox()
        {
            ParentOrigin = Mounts.CenterLeft;
            Origin = Mounts.CenterLeft;

            Position = new Vector2(10, -50);
            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Alpha = 0.8f,
                    Size = new Vector2(width, height),
                    Color = Color.Black
                },
                items = new Layer2D<ToolboxItem>
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Child = new ToolboxItem(new Enemy(null))
                }, 
            };
        }

        private class ToolboxItem : ClickableLayer<IDrawable2D>
        {
            private readonly Box background;
            private readonly Box flash;

            public ToolboxItem(IEditable editable)
            {
                Size = new Vector2(width * 0.8f, height / 8);
                Position = new Vector2(0, 8);

                ParentOrigin = Mounts.TopCenter;
                Origin = Mounts.TopCenter;

                Children = new IDrawable2D[]
                {
                    background = new Box
                    {
                        Name = "Background",
                        Alpha = 0.4f,
                        Size = Size,
                        Color = Color.DarkCyan,
                    },
                    flash = new Box
                    {
                        Name = "Flash",
                        Alpha = 0,
                        Size = Size,
                        Color = Color.White,
                    },
                    //editable.GetDrawable(),
                };
            }

            public override bool OnMouseDown(MouseButtonEvent e)
            {
                if (base.OnMouseDown(e))
                {
                    flash.Alpha = 1;
                    flash.FadeTo(0f, 200);
                    return true;
                }

                return false;
            }

            public override void OnHovered(MouseHoverEvent e)
            {
                base.OnHovered(e);
                background.FadeTo(1, 200);
            }

            public override void OnHoverLost(MouseHoverEvent e)
            {
                base.OnHoverLost(e);
                background.FadeTo(0.4f, 200);
            }
        }
    }
}