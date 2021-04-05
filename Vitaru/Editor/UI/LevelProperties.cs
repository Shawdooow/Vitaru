﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus;
using Vitaru.Levels;
using Vitaru.Themes;

namespace Vitaru.Editor.UI
{
    public class LevelProperties : InputLayer<IDrawable2D>
    {
        private const float width = 300;
        private const float height = 600;

        private readonly ListLayer<IDrawable2D> list;

        private readonly TextBox title;
        private readonly TextBox artist;
        private readonly TextBox filename;
        private readonly TextBox bpm;
        private readonly TextBox image;
        private readonly TextBox creator;
        private readonly TextBox level;

        public Action OnCreate;

        public LevelProperties()
        {
            Alpha = 0;

            ParentOrigin = Mounts.Center;
            Origin = Mounts.Center;

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
                list = new ListLayer<IDrawable2D>
                {
                    Size = new Vector2(width, height),

                    Spacing = 8,

                    Children = new IDrawable2D[]
                    {
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Format",
                            Color = ThemeManager.PrimaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.3f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Format
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Title",
                            Color = ThemeManager.PrimaryColor
                        },
                        title = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Metadata.Title
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Artist",
                            Color = ThemeManager.PrimaryColor
                        },
                        artist = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Metadata.Artist
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Filename",
                            Color = ThemeManager.PrimaryColor
                        },
                        filename = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Metadata.Filename
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "BPM",
                            Color = ThemeManager.PrimaryColor
                        },
                        bpm = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Metadata.BPM.ToString()
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Image Filename",
                            Color = ThemeManager.PrimaryColor
                        },
                        image = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Metadata.Image
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Creator (You)",
                            Color = ThemeManager.PrimaryColor
                        },
                        creator = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Creator
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },
                        new InstancedText
                        {
                            FontScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Level Name",
                            Color = ThemeManager.PrimaryColor
                        },
                        level = new TextBox
                        {
                            InstancedText =
                            {
                                FontScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.Name
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor
                        },

                        new Button
                        {
                            Name = "Create",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 40),

                            BackgroundSprite =
                            {
                                Color = ThemeManager.PrimaryColor
                            },

                            Text = "Create",
                            OnClick = create
                        }
                    }
                }
            };
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            PassDownInput = false;
        }

        private void create()
        {
            //Check to make sure we got good inputs
            foreach (IDrawable2D drawable in list)
                if (drawable is TextBox text && text.Text == null)
                    return;

            double b;

            try
            {
                b = double.Parse(bpm.Text);
            }
            catch
            {
                return;
            }

            LevelStore.CurrentLevel.Format =
                Vitaru.FEATURES >= Features.Experimental ? LevelStore.EXPERIMENTAL : LevelStore.STABLE;

            LevelStore.CurrentLevel.Metadata.Title = title.Text;
            LevelStore.CurrentLevel.Metadata.Artist = artist.Text;
            LevelStore.CurrentLevel.Metadata.Filename = filename.Text;
            LevelStore.CurrentLevel.Metadata.BPM = b;
            LevelStore.CurrentLevel.Metadata.Image = image.Text;
            LevelStore.CurrentLevel.Creator = creator.Text;
            LevelStore.CurrentLevel.Name = level.Text;

            OnCreate?.Invoke();
        }
    }
}