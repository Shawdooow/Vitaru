// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using System.Numerics;
using Vitaru.Roots;

namespace Vitaru.Wiki.Content
{
    public class WikiListLayer : ListLayer<IDrawable2D>
    {
        public WikiListLayer()
        {
            ParentOrigin = Mounts.Center;
            Origin = Mounts.Center;

            Size = new Vector2(WikiRoot.WIDTH - 40, WikiRoot.HEIGHT - 80);
            Spacing = 6;
        }
    }
}