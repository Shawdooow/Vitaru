// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Roots;

namespace Vitaru.Wiki.Content
{
    public class Description : Text2D
    {
        public Description(string description)
        {
            ParentOrigin = Mounts.TopLeft;
            Origin = Mounts.TopLeft;

            Text = description;
            FontScale = 0.24f;
            FixedWidth = WikiRoot.WIDTH - 40;
            X = 20;
        }
    }
}