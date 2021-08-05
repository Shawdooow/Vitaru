using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Screenshots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus.Debug;
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

        private readonly Layer2D<IDrawable2D> overlays;

        private byte[] pixels;
        private bool queued;

        public Camera(Layer2D<IDrawable2D> overlays)
        {
            this.overlays = overlays;

            Hitbox = new RectangularHitbox
            {
                Size = Size
            };

            Size = new Vector2(200, 120);

            Children = new IDrawable2D[]
            {
                CameraBox = new Box
                {
                    Size = Size,
                    Alpha = 0
                },
                new Corner
                {
                    ParentOrigin = Mounts.TopLeft
                },
                new Corner
                {
                    ParentOrigin = Mounts.TopRight,
                    Rotation = MathF.PI / 2
                },
                new Corner
                {
                    ParentOrigin = Mounts.BottomRight,
                    Rotation = MathF.PI
                },
                new Corner
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Rotation = MathF.PI / -2
                },
                xPos = new Text2D
                {
                    Position = new Vector2(-8, 8),
                    ParentOrigin = Mounts.TopRight,
                    Origin = Mounts.TopRight,
                    FontScale = 0.24f,
                    Alpha = 0.75f
                },
                yPos = new Text2D
                {
                    Position = new Vector2(-8),
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,
                    FontScale = 0.24f,
                    Alpha = 0.75f
                },
                xSize = new Text2D
                {
                    Position = new Vector2(8),
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.24f,
                    Alpha = 0.75f
                },
                ySize = new Text2D
                {
                    Position = new Vector2(8, -8),
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,
                    FontScale = 0.24f,
                    Alpha = 0.75f
                },
            };
        }

        public override void PreRender()
        {
            base.PreRender();

            xPos.Text = "x: " + (int)X;
            yPos.Text = "y: " + (int)Y;
            xSize.Text = "w: " + (int)Width;
            ySize.Text = "h: " + (int)Height;
        }

        public override void Render()
        {
            if (queued)
            {
                GCHandle h = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                IntPtr ptr = h.AddrOfPinnedObject();

                Renderer.Screenshot(new ScreenshotParamaters
                {
                    X = (int)X,
                    Y = (int)Y,

                    Width = (int)Width,
                    Height = (int)Height,

                    Ptr = ptr
                });

                Texture texture = Renderer.Context.BufferPixels(pixels, (int)Width, (int)Height, "Screenshot", true);

                overlays.Add(new Sprite(texture));

                queued = false;
            }

            base.Render();
        }

        private class Corner : Layer2D<Box>
        {
            private const int height = 5;
            private const int width = 16;

            internal Corner()
            {
                Children = new Box[]
                {
                    new()
                    {
                        ParentOrigin = Mounts.TopLeft,
                        Origin = Mounts.TopLeft,
                        Size = new Vector2(width, height),
                        Color = Color.White
                    },
                    new()
                    {
                        ParentOrigin = Mounts.TopLeft,
                        Origin = Mounts.TopLeft,
                        Size = new Vector2(height, width),
                        Color = Color.White
                    }
                };
            }
        }

        public void QueueScreenshot()
        {
            if (queued) return;

            pixels = new byte[(int)Width * (int)Height * 4];

            queued = true;
        }
    }
}
