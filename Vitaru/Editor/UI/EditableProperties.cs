// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Color;
using Vitaru.Editor.Editables.Properties.Pattern;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Editor.Editables.Properties.Time;
using Vitaru.Themes;

namespace Vitaru.Editor.UI
{
    public class EditableProperties : InputLayer<IDrawable2D>
    {
        private const float width = 160;
        private const float height = 400;

        private readonly LevelManager manager;

        private SpriteText name;
        private ListLayer<IDrawable2D> properties;

        public EditableProperties(LevelManager manager)
        {
            this.manager = manager;
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
                    Origin = Mounts.TopCenter
                }
            };

            manager.PropertiesSet += ps =>
            {
                properties.ClearChildren();

                if (ps != null)
                {
                    foreach (EditableProperty p in ps)
                    {
                        TextBox s;
                        switch (p)
                        {
                            default:
                                continue;
                            case EditableStartPosition startPos:

                                TextBox x;
                                TextBox y;

                                properties.AddArray(new IDrawable2D[]
                                {
                                    new SpriteText
                                    {
                                        TextScale = 0.3f,
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = "Position (x, y)"
                                    },
                                    x = new TextBox
                                    {
                                        SpriteText =
                                        {
                                            TextScale = 0.25f
                                        },

                                        Size = new Vector2(width - 10, 16),
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = startPos.Value.X.ToString(),
                                        OnEnter = s =>
                                        {
                                            float n;
                                            try
                                            {
                                                n = float.Parse(s);
                                            }
                                            catch
                                            {
                                                n = startPos.Value.X;
                                            }

                                            startPos.SetValue(new Vector2(n, startPos.Value.Y));
                                        }
                                    },
                                    y = new TextBox
                                    {
                                        SpriteText =
                                        {
                                            TextScale = 0.25f
                                        },

                                        Size = new Vector2(width - 10, 16),
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = startPos.Value.Y.ToString(),
                                        OnEnter = s =>
                                        {
                                            float n;
                                            try
                                            {
                                                n = float.Parse(s);
                                            }
                                            catch
                                            {
                                                n = startPos.Value.Y;
                                            }

                                            startPos.SetValue(new Vector2(startPos.Value.X, n));
                                        }
                                    },
                                    new Box
                                    {
                                        Name = "Spacer",
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Size = new Vector2(width - 4, 2),
                                        Color = ThemeManager.SecondaryColor
                                    }
                                });

                                startPos.OnValueUpdated += pos => x.Text = pos.X.ToString();
                                startPos.OnValueUpdated += pos => y.Text = pos.Y.ToString();
                                continue;
                            case EditableStartTime startTime:

                                properties.AddArray(new IDrawable2D[]
                                {
                                    new SpriteText
                                    {
                                        TextScale = 0.3f,
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = "Start Time"
                                    },
                                    s = new TextBox
                                    {
                                        SpriteText =
                                        {
                                            TextScale = 0.25f
                                        },

                                        Size = new Vector2(width - 10, 16),
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = startTime.Value.ToString(),
                                        OnEnter = t =>
                                        {
                                            double n;
                                            try
                                            {
                                                n = double.Parse(t);
                                            }
                                            catch
                                            {
                                                n = startTime.Value;
                                            }

                                            startTime.SetValue(n);
                                        }
                                    },
                                    new Box
                                    {
                                        Name = "Spacer",
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Size = new Vector2(width - 4, 2),
                                        Color = ThemeManager.SecondaryColor
                                    }
                                });

                                startTime.OnValueUpdated += time => s.Text = time.ToString();
                                continue;
                            case EditableColor color:
                                properties.AddArray(new IDrawable2D[]
                                {
                                    new SpriteText
                                    {
                                        TextScale = 0.3f,
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = "Color (WIP)"
                                    },
                                    new Box
                                    {
                                        Name = "Spacer",
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Size = new Vector2(width - 4, 2),
                                        Color = ThemeManager.SecondaryColor
                                    }
                                });
                                continue;
                            case EditablePatternID id:

                                properties.AddArray(new IDrawable2D[]
                                {
                                    new SpriteText
                                    {
                                        TextScale = 0.3f,
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = "Pattern ID"
                                    },
                                    s = new TextBox
                                    {
                                        SpriteText =
                                        {
                                            TextScale = 0.25f
                                        },

                                        Size = new Vector2(width - 10, 16),
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Text = id.Value.ToString(),
                                        OnEnter = t =>
                                        {
                                            short n;
                                            try
                                            {
                                                n = short.Parse(t);
                                                n = Math.Clamp(n, (short) 0, (short) 4);
                                            }
                                            catch
                                            {
                                                n = id.Value;
                                            }

                                            id.SetValue(n);
                                        }
                                    },
                                    new Box
                                    {
                                        Name = "Spacer",
                                        ParentOrigin = Mounts.TopCenter,
                                        Origin = Mounts.TopCenter,
                                        Size = new Vector2(width - 4, 2),
                                        Color = ThemeManager.SecondaryColor
                                    }
                                });

                                id.OnValueUpdated += time => s.Text = time.ToString();
                                continue;
                        }
                    }

                    name.Text = manager.SelectedEditable.Name;
                }
                else
                    name.Text = "None";
            };

            base.LoadingComplete();
        }

        public void Selected(IEditable editable)
        {
            manager.SetProperties(editable?.GetProperties());
        }
    }
}