// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Networking.Client;

namespace Vitaru.Roots.Multi
{
    public class Lobby : MultiRoot
    {
        protected override bool Parallax => true;

        public Lobby(VitaruNetHandler vitaruNet) : base(vitaruNet)
        {
            AddArray(new ILayer[]
            {
                new InstancedText(10, true)
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,

                    Position = new Vector2(10),
                    Text = "Rooms:"
                },
                new Rooms()
            });
        }

        public class Rooms : Layer2D<IDrawable2D>
        {
            public Rooms()
            {

            }
        }
    }
}