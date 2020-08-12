using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Levels;

namespace Vitaru.Editor.UI
{
    public class LevelProperties : InputLayer<IDrawable2D>
    {
        private const float width = 200;
        private const float height = 600;

        public LevelProperties()
        {
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
                    Spacing = 2,

                    Children = new IDrawable2D[]
                    {
                        new SpriteText
                        {
                            TextScale = 0.3f,
                            X = 10,
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = "Title"
                        }, 
                        new TextBox
                        {
                            SpriteText =
                            {
                                TextScale = 0.25f
                            },

                            X = 10,
                            Size = new Vector2(width - 20, 16),
                            ParentOrigin = Mounts.TopLeft,
                            Origin = Mounts.TopLeft,
                            Text = LevelStore.CurrentLevel.LevelTrack.Title
                        }
                    }
                }, 
            };
        }
    }
}
