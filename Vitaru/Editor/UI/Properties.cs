// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Editor.IO;

namespace Vitaru.Editor.UI
{
    public class Properties : InputLayer<IDrawable2D>
    {
        private const float width = 140;
        private const float height = 400;

        private readonly SpriteText name;

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

                    TextScale = 0.25f,
                }
            };
        }

        public void Selected(Editable edit)
        {
            editable = edit;
            IEditable e = editable.GetEditable(null);

            name.Text = e.Name;
        }
    }
}