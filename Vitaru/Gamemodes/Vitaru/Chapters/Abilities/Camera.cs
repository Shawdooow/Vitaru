using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Abilities
{
    public class Camera : Layer2D<IDrawable2D>
    {
        public Box CameraBox;
        public RectangularHitbox Hitbox;

        private readonly Text2D xPos;
        private readonly Text2D yPos;

        private readonly Text2D xSize;
        private readonly Text2D ySize;

        public Camera()
        {
            Hitbox = new RectangularHitbox
            {
                Size = Size
            };

            Origin = Mounts.Center;
            Size = new Vector2(200, 120);

            Children = new IDrawable2D[]
            {
                CameraBox = new Box
                {
                    Size = Size
                },
                new Corner(),
                new Corner
                {
                    ParentOrigin = Mounts.TopRight,
                    Rotation = 90
                },
                new Corner
                {
                    ParentOrigin = Mounts.BottomRight,
                    Rotation = 180
                },
                new Corner
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Rotation = 270
                },
                xPos = new Text2D
                {
                    Position = new Vector2(-8, 8),
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.TopRight,
                    FontScale = 0.36f,
                    Alpha = 0.75f
                },
                yPos = new Text2D
                {
                    Position = new Vector2(-8),
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,
                    FontScale = 0.36f,
                    Alpha = 0.75f
                },
                xSize = new Text2D
                {
                    Position = new Vector2(8),
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.36f,
                    Alpha = 0.75f
                },
                ySize = new Text2D
                {
                    Position = new Vector2(8, -8),
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,
                    FontScale = 0.36f,
                    Alpha = 0.75f
                },
            };
        }

        public override void PreRender()
        {
            base.PreRender();

            xPos.Text = "x: " + (int)CameraBox.DrawTransform.M14;
            yPos.Text = "y: " + (int)CameraBox.DrawTransform.M24;
            xSize.Text = "w: " + (int)CameraBox.DrawTransform.M11;
            ySize.Text = "h: " + (int)CameraBox.DrawTransform.M22;
        }

        private class Corner : Layer2D<Box>
        {
            private const int height = 5;
            private const int width = 16;

            internal Corner()
            {
                Children = new Box[]
                {
                    new Box
                    {
                        Size = new Vector2(width, height),
                        Color = Color.White
                    },
                    new Box
                    {
                        Size = new Vector2(height, width),
                        Color = Color.White
                    }
                };
            }
        }
    }
}
