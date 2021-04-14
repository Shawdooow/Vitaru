using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.Debug;
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
        }

        private void select(SelectableCharacter item)
        {
            foreach (SelectableCharacter i in items)
                i.DeSelect();

            item.Select();
            Logger.Log("Set Character HERE!!", Output.SystemConsole);
        }

        public class SelectableCharacter : ClickableLayer<IDrawable2D>
        {
            private readonly Box background;
            private readonly Box flash;

            public SelectableCharacter(Player character, int index)
            {
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
                    },
                    new InstancedText(character.Name.Length)
                    {
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,
                        Text = character.Name,
                        FontScale = 0.25f
                    }
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
