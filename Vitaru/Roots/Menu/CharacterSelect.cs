using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input.Events;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Players;

namespace Vitaru.Roots.Menu
{
    public class CharacterSelect : InputLayer<IDrawable2D>
    {
        private const float width = 320;
        private const float height = 640;

        private readonly InputLayer<SelectableCharacter> items;

        public CharacterSelect()
        {
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
                items = new InputLayer<SelectableCharacter>
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft
                }
            };

            for (int i = 0; i < GamemodeStore.SelectedGamemode.Players.Count; i++)
            {
                SelectableCharacter item = new(GamemodeStore.SelectedGamemode.Players[i], i);
                item.OnClick = () => select(item);
                items.Add(item);
            }

            select(items.Children[0]);
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
            private Sprite sign;

            public SelectableCharacter(KeyValuePair<Chapter, Player> pair, int index)
            {
                this.pair = pair;

                ParentOrigin = Mounts.TopLeft;
                Origin = Mounts.TopLeft;

                Size = new Vector2(width / 2);
                Position = new Vector2(width / 2 * (index % 2), width / 2 * MathF.Round(index / 2f, MidpointRounding.ToZero));

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
                    }
                };
            }

            public override void LoadingComplete()
            {
                base.LoadingComplete();

                DrawableGameEntity drawable = pair.Value.GenerateDrawable();
                drawable.Position = Vector2.Zero;
                AddArray(new IDrawable2D[]
                {
                    drawable,
                    new Text2D(pair.Value.Name.Length)
                    {
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,
                        Y = 2,
                        Text = pair.Value.Name,
                        FontScale = 0.3f
                    }
                });

                if (drawable is DrawablePlayer p)
                    sign = p.Seal.Sign;
            }

            public override void PreRender()
            {
                base.PreRender();
                sign.Rotation += (float)(Clock.LastElapsedTime / 1000);
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
