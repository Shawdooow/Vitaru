// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Themes;

namespace Vitaru.Editor.UI
{
    public class EditableProperties : InputLayer<IDrawable2D>
    {
        private const float width = 160;
        private const float height = 400;

        private SpriteText name;
        private ListLayer<IDrawable2D> properties;

        public EditableProperties(LevelManager manager)
        {
            manager.EditableSet += Selected;

            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Position = new Vector2(-10, -50);
            Size = new Vector2(width, height);
        }

        public override void LoadingComplete()
        {
            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Alpha = 0.8f,
                    ParentSizing = Axes.Both,
                    Color = Color.Black
                },
                name = new SpriteText
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    TextScale = 0.4f
                },
                properties = new ListLayer<IDrawable2D>
                {
                    Y = 36,
                    ParentSizing = Axes.Both,
                    Spacing = 4,
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,
                }
            };

            base.LoadingComplete();
        }

        public void Selected(IEditable editable)
        {
            EditableProperty[] ps = editable.GetProperties();

            properties.ClearChildren();

            foreach (EditableProperty p in ps)
            {
                switch (p)
                {
                    default:
                        continue;
                    case EditableStartPosition startPos:
                        properties.AddArray(new IDrawable2D[]
                        {
                            new SpriteText
                            {
                                TextScale = 0.3f,
                                ParentOrigin = Mounts.TopCenter,
                                Origin = Mounts.TopCenter,
                                Text = "Position (x, y)"
                            },
                            new TextBox
                            {
                                SpriteText =
                                {
                                    TextScale = 0.25f
                                },

                                Size = new Vector2(width - 10, 16),
                                ParentOrigin = Mounts.TopCenter,
                                Origin = Mounts.TopCenter,
                                Text = startPos.Value.X.ToString()
                            },
                            new TextBox
                            {
                                SpriteText =
                                {
                                    TextScale = 0.25f
                                },

                                Size = new Vector2(width - 10, 16),
                                ParentOrigin = Mounts.TopCenter,
                                Origin = Mounts.TopCenter,
                                Text = startPos.Value.Y.ToString()
                            },
                            new Box
                            {
                                Name = "Spacer",
                                ParentOrigin = Mounts.TopCenter,
                                Origin = Mounts.TopCenter,
                                Size = new Vector2(width - 4, 2),
                                Color = ThemeManager.SecondaryColor,
                            }
                        });
                        continue;
                }
            }

            name.Text = editable.Name;
        }
    }
}