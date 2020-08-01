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

namespace Vitaru.Editor.UI
{
    public class Properties : InputLayer<IDrawable2D>
    {
        private const float width = 160;
        private const float height = 400;

        private readonly SpriteText name;
        private readonly InputLayer<IDrawable2D> properties;

        private Editable editable;

        public Properties()
        {
            ParentOrigin = Mounts.CenterRight;
            Origin = Mounts.CenterRight;

            Position = new Vector2(-10, -50);
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
                name = new SpriteText
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,

                    TextScale = 0.4f
                },
                properties = new InputLayer<IDrawable2D>
                {
                    Position = new Vector2(0, 10),
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft
                }
            };
        }

        public void Selected(Editable edit)
        {
            editable = edit;
            IEditable e = editable.GetEditable(null);

            EditableProperty[] ps = e.GetProperties();

            properties.ClearChildren();

            foreach (EditableProperty p in ps)
            {
                switch (p)
                {
                    default:
                        continue;
                    case EditableVector2 vec2:
                        properties.Add(new SpriteText
                        {
                            TextScale = 0.3f,
                            Position = new Vector2(0, 36),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Position (x, y)"
                        });
                        properties.Add(new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.25f
                            },

                            Size = new Vector2(80, 16),
                            Position = new Vector2(0, 50),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = vec2.Value.X.ToString()
                        });
                        properties.Add(new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.25f
                            },

                            Size = new Vector2(80, 16),
                            Position = new Vector2(80, 50),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = vec2.Value.Y.ToString()
                        });
                        continue;
                }
            }

            name.Text = e.Name;
        }
    }
}