// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Input.Events;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Editor.UI
{
    public class Toolbox : InputLayer<IDrawable2D>
    {
        public override string Name => "Toolbox";

        private const float width = 140;
        private const float height = 400;

        private readonly InputLayer<ToolboxItem> items;

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
                items = new InputLayer<ToolboxItem>
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                    Children = new[]
                    {
                        new ToolboxItem(new Enemy(null), 0)
                        {
                            OnClick = () => select(0)
                        },
                        new ToolboxItem(new Bullet
                        {
                            Color = Color.GreenYellow
                        }, 1)
                        {
                            OnClick = () => select(1)
                        }
                    }
                }
            };
        }

        private void select(int index)
        {
            foreach (ToolboxItem item in items)
                item.DeSelect();

            items.Children[index].Select();
        }

        private class ToolboxItem : ClickableLayer<IDrawable2D>
        {
            private readonly Box background;
            private readonly Box flash;

            public ToolboxItem(IEditable editable, int index)
            {
                Size = new Vector2(width * 0.86f, height / 8);
                Position = new Vector2(0, 8 * (index + 1) + height / 8 * index);

                ParentOrigin = Mounts.TopCenter;
                Origin = Mounts.TopCenter;

                Layer2D<IDrawable2D> draw = editable.GetDrawable();

                draw.ParentOrigin = Mounts.CenterLeft;
                draw.Origin = Mounts.CenterLeft;
                draw.Size = new Vector2(height / 8);

                Children = new IDrawable2D[]
                {
                    background = new Box
                    {
                        Name = "Background",
                        Alpha = 0.4f,
                        Size = Size,
                        Color = Color.DarkCyan
                    },
                    flash = new Box
                    {
                        Name = "Flash",
                        Alpha = 0,
                        Size = Size,
                        Color = Color.White
                    },
                    new SpriteText
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterRight,
                        Position = new Vector2(-2, 0),
                        Text = editable.Name,
                        TextScale = 0.2f
                    },
                    draw
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

            public void Select()
            {
                background.Color = Color.GreenYellow;
            }

            public void DeSelect()
            {
                background.Color = Color.DarkCyan;
            }
        }
    }
}