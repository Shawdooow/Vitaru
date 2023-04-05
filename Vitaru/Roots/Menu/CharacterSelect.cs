// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Chapters;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Roots.Menu
{
    public class CharacterSelect : HoverableLayer<IDrawable2D>
    {
        private const float width = 160 * 4;
        private const float height = 160 * 5;

        private readonly InputLayer<SelectableCharacter> items;

        public CharacterSelect()
        {
            Y = -16;
            Size = new Vector2(width, height);

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Alpha = 0.8f,
                    Size = new Vector2(width, height),
                    Color = Color.Black,
                },
                items = new InputLayer<SelectableCharacter>
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                },
            };

            for (int i = 0; i < GamemodeStore.SelectedGamemode.Players.Count; i++)
            {
                SelectableCharacter item = new(GamemodeStore.SelectedGamemode.Players[i], i);
                item.OnClick = () => select(item);
                items.Add(item);
            }

            select(items.Children[0]);
        }

        public override void OnHovered()
        {
            base.OnHovered();

            if (Renderer.CurrentRoot.Cursor != null)
                Renderer.CurrentRoot.Cursor.Hover(Color.GreenYellow);
        }

        public override void OnHoverLost()
        {
            base.OnHoverLost();

            if (Renderer.CurrentRoot.Cursor != null)
                Renderer.CurrentRoot.Cursor.HoverLost();
        }

        private void select(SelectableCharacter item)
        {
            foreach (SelectableCharacter i in items)
                i.DeSelect();

            item.Select();
        }

        public class SelectableCharacter : ClickableLayer<IDrawable2D>
        {
            private readonly Box background;
            private readonly Box flash;

            private readonly KeyValuePair<Chapter, Player> pair;
            private DrawablePlayer drawable;

            public SelectableCharacter(KeyValuePair<Chapter, Player> pair, int index)
            {
                this.pair = pair;

                ParentOrigin = Mounts.TopLeft;
                Origin = Mounts.TopLeft;

                Size = new Vector2(width / 4);
                Position = new Vector2(width / 4 * (index % 4),
                    width / 4 * MathF.Round(index / 4f, MidpointRounding.ToZero));

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
                };
            }

            public override void LoadingComplete()
            {
                base.LoadingComplete();

                //Load the Drawable here
                drawable = new DrawablePlayer(pair.Value, this);
                drawable.Position = Vector2.Zero;
                drawable.HitboxAlpha = 1;

                AddArray(new IDrawable2D[]
                {
                    new Text2D
                    {
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,
                        Y = 2,
                        Text = pair.Value.Name,
                        FontScale = 0.3f,
                    },
                });

                if (pair.Value.ImplementationState < ImplementationState.Complete)
                    Add(new Text2D
                    {
                        Position = new Vector2(-4, -12),
                        Text = "WIP",
                        Alpha = 0.8f,
                        Color = pair.Value.ImplementationState < ImplementationState.MostlyComplete ? 
                            pair.Value.ImplementationState < ImplementationState.PartiallyComplete ? 
                            Color.Black : Color.Red : Color.Yellow,
                        FontScale = 1f,
                    });
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
                GamemodeStore.SelectedGamemode.SelectedCharacter = $"{pair.Key.Title}:{pair.Value.Name}";
            }

            public void DeSelect()
            {
                background.Color = Color.DarkCyan;
            }
        }
    }
}