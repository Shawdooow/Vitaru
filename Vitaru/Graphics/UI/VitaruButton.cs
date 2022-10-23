using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Utilities;

namespace Vitaru.Graphics.UI
{
    public  class VitaruButton : Button
    {
        public override Vector2 Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                Border.Size = value;
            }
        }

        public override Color Color
        {
            get => Border.Color;
            set
            {
                Border.Color = value;
                BackgroundSprite.Color = value.Darker(0.5f);
            }
        }

        public readonly ButtonBorder Border = new ButtonBorder();

        public override void LoadingComplete()
        {
            Remove(Text2D, false);
            Add(Border);
            Add(Text2D);
            base.LoadingComplete();
        }

        public class ButtonBorder : Layer2D<Box>
        {
            const int w = 2;

            public override Vector2 Size
            {
                get => base.Size;
                set
                {
                    base.Size = value;

                    ProtectedChildren[0].Width = value.X + w;
                    ProtectedChildren[0].Y = -value.Y / 2;

                    ProtectedChildren[1].Width = value.X + w;
                    ProtectedChildren[1].Y = value.Y / 2;

                    ProtectedChildren[2].Height = value.Y + w;
                    ProtectedChildren[2].X = -value.X / 2;

                    ProtectedChildren[3].Height = value.Y + w;
                    ProtectedChildren[3].X = value.X / 2;
                }
            }

            public ButtonBorder()
            {
                Children = new[]
                {
                    new Box
                    {
                        Height = w,
                    },
                    new Box
                    {
                        Height = w,
                    },
                    new Box
                    {
                        Width = w,
                    },
                    new Box
                    {
                        Width = w,
                    },
                };
            }
        }
    }
}
