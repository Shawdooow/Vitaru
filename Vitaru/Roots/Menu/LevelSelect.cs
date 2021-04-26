using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input.Events;
using Vitaru.Levels;

namespace Vitaru.Roots.Menu
{
    public class LevelSelect : Layer2D<IDrawable2D>
    {
        private const float width = 160;
        private const float height = 200;

        private readonly InputLayer<LevelItem> items;

        public LevelSelect()
        {
            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Position = new Vector2(-10, 0);
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
                items = new InputLayer<LevelItem>
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter
                }
            };

            LevelStore.OnPackChange += setButtons;
            setButtons(LevelStore.CurrentPack);
        }

        public override void PreRender()
        {
            base.PreRender();

            if (pack != null)
            {
                items.ClearChildren();

                LevelItem r = new(null, -1, "Random");
                r.OnClick = () => select(r);
                items.Add(r);

                for (int i = 0; i < pack.Levels.Length; i++)
                {
                    LevelItem item = new(pack, i, pack.Levels[i].Name);
                    item.OnClick = () => select(item);
                    items.Add(item);
                }

                pack = null;
                select(items.Children[0]);
            }
        }

        private LevelPack pack;

        private void setButtons(LevelPack p) => pack = p;

        private void select(LevelItem item)
        {
            foreach (LevelItem i in items)
                i.DeSelect();

            item.Select();
            LevelStore.SetLevel(item.Pack, item.Index);
        }

        protected override void Dispose(bool finalize)
        {
            LevelStore.OnPackChange -= setButtons;
            base.Dispose(finalize);
        }

        private class LevelItem : ClickableLayer<IDrawable2D>
        {
            public readonly LevelPack Pack;
            public readonly int Index;

            private readonly Box background;
            private readonly Box flash;

            public LevelItem(LevelPack pack, int index, string name)
            {
                Pack = pack;
                Index = index;

                index++;

                Size = new Vector2(width * 0.86f, height / 4);
                Position = new Vector2(0, 4 * (index + 1) + height / 4 * index);

                ParentOrigin = Mounts.TopCenter;
                Origin = Mounts.TopCenter;

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
                    new Text2D
                    {
                        ParentOrigin = Mounts.CenterRight,
                        Origin = Mounts.CenterRight,
                        Position = new Vector2(-2, 0),
                        Text = name,
                        FontScale = 0.2f
                    },
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
