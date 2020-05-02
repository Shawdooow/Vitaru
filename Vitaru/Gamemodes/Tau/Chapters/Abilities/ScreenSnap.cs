// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#endregion

// ReSharper disable InconsistentNaming
#pragma warning disable 4014

namespace Vitaru.Gamemodes.Tau.Chapters.Abilities
{
    public class ScreenSnap : Sprite
    {
        private GameHost host;

        private readonly Box area;

        private static int img_count;
        private int imgCount;

        private static ResourceStore<byte[]> img_resources;
        private static TextureStore img_textures;

        public ScreenSnap(Box area)
        {
            Alpha = 0;
            AlwaysPresent = true;
            this.area = area;
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame osu, OsuConfigManager config, GameHost host, Storage storage)
        {
            this.host = host;

            snap(storage, config.GetBindable<ScreenshotFormat>(OsuSetting.ScreenshotFormat));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Texture = img_textures?.Get("snapshot" + imgCount + ".png") ??
                      img_textures?.Get("snapshot" + imgCount + ".jpeg");
        }

        private async Task snap(Storage storage, Bindable<ScreenshotFormat> screenshotFormat) => await Task.Run(
            async () =>
            {
                Rectangle rect = new Rectangle(new Point(0, 0),
                    new Size((int) area.ScreenSpaceDrawQuad.Width, (int) area.ScreenSpaceDrawQuad.Height));

                using (Bitmap bitmap = await snapshot(rect))
                {
                    switch (screenshotFormat.Value)
                    {
                        case ScreenshotFormat.Png:
                            bitmap.Save(
                                storage.GetStream("vitaru\\temp\\snapshot" + img_count + ".png", FileAccess.Write,
                                    FileMode.Create), ImageFormat.Png);
                            imgCount = img_count;
                            img_count++;
                            break;
                        case ScreenshotFormat.Jpg:
                            bitmap.Save(
                                storage.GetStream("vitaru\\temp\\snapshot" + img_count + ".jpeg", FileAccess.Write,
                                    FileMode.Create), ImageFormat.Jpeg);
                            imgCount = img_count;
                            img_count++;
                            break;
                    }

                    if (img_resources == null)
                    {
                        img_resources =
                            new ResourceStore<byte[]>(
                                new StorageBackedResourceStore(storage.GetStorageForDirectory("vitaru\\temp")));
                        img_textures = new TextureStore(new TextureLoaderStore(img_resources));
                    }
                }
            });

        /// <summary>
        ///     FROM: osu.Framework.Platform.GameHost
        /// </summary>
        private async Task<Bitmap> snapshot(Rectangle rectangle)
        {
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            BitmapData data = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            bool complete = false;

            host.DrawThread.Scheduler.Add(() =>
            {
                if (GraphicsContext.CurrentContext == null)
                    throw new GraphicsContextMissingException();

                osuTK.Graphics.OpenGL.GL.ReadPixels((int) area.ScreenSpaceDrawQuad.TopLeft.X,
                    (int) area.ScreenSpaceDrawQuad.TopLeft.Y, rectangle.Width, rectangle.Height,
                    osuTK.Graphics.OpenGL.PixelFormat.Bgr, osuTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
                complete = true;
            });

            await Task.Run(() =>
            {
                while (!complete)
                    Thread.Sleep(50);
            });

            bitmap.UnlockBits(data);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bitmap;
        }
    }
}
#pragma warning restore 4014