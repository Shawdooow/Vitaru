// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Mitochondria.Input.Events;
using Vitaru.Editor.Editables;
using Vitaru.Gamemodes;

namespace Vitaru.Editor.UI
{
    public class Toolbox : InputLayer<IDrawable2D>
    {
        public override string Name => nameof(Toolbox);

        private const float width = 160;
        private const float height = 400;

        private readonly LevelManager manager;

        private readonly InputLayer<ToolboxItem> items;

        public Toolbox(LevelManager manager)
        {
            this.manager = manager;

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
                    Origin = Mounts.TopCenter
                }
            };

            EditableGenerator[] generators = GamemodeStore.SelectedGamemode.Gamemode.GetGenerators();
            for (int i = 0; i < generators.Length; i++)
            {
                ToolboxItem item = new ToolboxItem(generators[i], i);
                item.OnClick = () => select(item);
                items.Add(item);
            }
        }

        private void select(ToolboxItem item)
        {
            foreach (ToolboxItem i in items)
                i.DeSelect();

            item.Select();
            manager.SetGenerator(item.Generator);
        }

        private class ToolboxItem : ClickableLayer<IDrawable2D>
        {
            public readonly EditableGenerator Generator;

            private readonly Box background;
            private readonly Box flash;

            public ToolboxItem(EditableGenerator generator, int index)
            {
                Generator = generator;

                Size = new Vector2(width * 0.86f, height / 8);
                Position = new Vector2(0, 8 * (index + 1) + height / 8 * index);

                ParentOrigin = Mounts.TopCenter;
                Origin = Mounts.TopCenter;

                IEditable edit = generator.GetEditable(null);
                DrawableGameEntity draw = edit.GenerateDrawable();
                edit.SetDrawable(draw);

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
                    new InstancedText
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterRight,
                        Position = new Vector2(-2, 0),
                        Text = edit.Name,
                        FontScale = 0.2f
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

            public override void OnHovered()
            {
                base.OnHovered();
                background.FadeTo(1, 200);
            }

            public override void OnHoverLost()
            {
                base.OnHoverLost();
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