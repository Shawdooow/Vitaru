// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Levels;
using Vitaru.Themes;

namespace Vitaru.Editor.UI
{
    public class LevelProperties : InputLayer<IDrawable2D>
    {
        private const float width = 300;
        private const float height = 600;

        public LevelProperties()
        {
            Alpha = 0;
            PassDownInput = false;

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
                new ListLayer<IDrawable2D>
                {
                    Size = new Vector2(width, height),

                    Spacing = 8,

                    Children = new IDrawable2D[]
                    {
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Format",
                            Color = ThemeManager.PrimaryColor,
                        },
                        new SpriteText
                        {
                            TextScale = 0.3f,
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
                            Color = ThemeManager.SecondaryColor,
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Title",
                            Color = ThemeManager.PrimaryColor,
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.LevelTrack.Title
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor,
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Artist",
                            Color = ThemeManager.PrimaryColor,
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.LevelTrack.Artist
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor,
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Creator (You)",
                            Color = ThemeManager.PrimaryColor,
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.LevelCreator
                        },
                        new Box
                        {
                            Name = "Spacer",
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Size = new Vector2(width - 20, 2),
                            Color = ThemeManager.SecondaryColor,
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = "Level Name",
                            Color = ThemeManager.PrimaryColor,
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            Size = new Vector2(width - 60, 16),
                            ParentOrigin = Mounts.TopCenter,
                            Origin = Mounts.TopCenter,
                            Text = LevelStore.CurrentLevel.LevelName
                        },
                    }
                },
            };
        }
    }
}