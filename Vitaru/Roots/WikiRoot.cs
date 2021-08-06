using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Roots.Menu;
using Vitaru.Tracks;
using Vitaru.Wiki;

namespace Vitaru.Roots
{
    public class WikiRoot : MenuRoot
    {
        public override string Name => nameof(WikiRoot);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private WikiPanel panel;

        public const float WIDTH = 1200;
        public const float HEIGHT = 800;

        private Text2D title;
        private ListLayer<SectionButton> sections;
        private CachedLayer<IDrawable2D> content;

        private VitaruTrackController controller;

        public WikiRoot()
        {
            Index index = new();
            Add(index);
            Add(new Layer2D<IDrawable2D>
            {
                Size = new Vector2(WIDTH, HEIGHT),

                Children = new IDrawable2D[]
                {
                    title = new Text2D
                    {
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.BottomCenter,
                        Y = -8,
                        FontScale = 0.64f,
                        Text = "Wiki"
                    },
                    new Box
                    {
                        Alpha = 0.8f,
                        Size = new Vector2(WIDTH, HEIGHT),
                        Color = Color.Black
                    },
                    sections = new ListLayer<SectionButton>
                    {
                        ParentOrigin = Mounts.TopCenter,
                        Origin = Mounts.TopCenter,

                        Size = new Vector2(WIDTH, 40),
                        Direction = Direction.Horizontal
                    },
                    content = new CachedLayer<IDrawable2D>
                    {
                        ParentOrigin = Mounts.BottomCenter,
                        Origin = Mounts.BottomCenter,

                        Size = new Vector2(WIDTH, HEIGHT - 40),
                    }
                }
            });
            Add(new Version());

            index.OnSetPanel += p => panel = p;
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight,

                PassDownInput = false,
                Alpha = 0
            });
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryNextLevel();
        }

        public override void PreRender()
        {
            base.PreRender();

            if (panel != null)
            {
                title.Text = panel.Name;

                sections.ClearChildren();
                WikiSection[] s = panel.GetSections();
                foreach (WikiSection section in s)
                {
                    SectionButton button = new(section);
                    button.OnClick = () =>
                    {
                        foreach (SectionButton b in sections)
                            b.DeSelect();

                        button.Select();
                        content.Child = section.GetSection();
                    };
                    sections.Add(button);
                }

                sections.Children[0].Select();
                content.Child = s[0].GetSection();

                panel = null;
            }
        }

        private class SectionButton : Button
        {
            public SectionButton(WikiSection section)
            {
                ParentOrigin = Mounts.CenterLeft;
                Origin = Mounts.CenterLeft;

                Size = new Vector2(WIDTH / 6f, 40);

                Text = section.Name;
                Text2D.FontScale = 0.48f;

                Remove(BackgroundSprite);

                Dim.Alpha = 0.4f;
                Dim.Color = Color.DarkCyan;
            }

            public void Select()
            {
                Dim.Color = Color.GreenYellow;
            }

            public void DeSelect()
            {
                Dim.Color = Color.DarkCyan;
            }
        }
    }
}
