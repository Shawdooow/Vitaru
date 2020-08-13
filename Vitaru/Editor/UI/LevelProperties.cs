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
                            X = 10,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Title"
                        }, 
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            X = 10,
                            Size = new Vector2(width - 20, 16),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = LevelStore.CurrentLevel.LevelTrack.Title
                        },
                        new Box
                        {
                            Name = "Spacer",
                            X = 30,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Size = new Vector2(width - 60, 2),
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            X = 10,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Artist"
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            X = 10,
                            Size = new Vector2(width - 20, 16),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = LevelStore.CurrentLevel.LevelTrack.Artist
                        },                        
                        new Box
                        {
                            Name = "Spacer",
                            X = 30,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Size = new Vector2(width - 60, 2),
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            X = 10,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Creator (You)"
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            X = 10,
                            Size = new Vector2(width - 20, 16),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = LevelStore.CurrentLevel.LevelCreator
                        },
                        new Box
                        {
                            Name = "Spacer",
                            X = 30,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Size = new Vector2(width - 60, 2),
                        },
                        new SpriteText
                        {
                            TextScale = 0.35f,
                            X = 10,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Level Name"
                        },
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.3f
                            },

                            X = 10,
                            Size = new Vector2(width - 20, 16),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = LevelStore.CurrentLevel.LevelName
                        },
                    }
                }, 
            };
        }
    }
}
