// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;

namespace Vitaru.Roots.Multi
{
    public class Lobby : MultiRoot
    {
        protected override bool Parallax => true;

        private TrackController controller;

        public Lobby(Pack<Updatable> networking) : base(networking)
        {
            AddArray(new ILayer[]
            {
                new Text2D(10, true)
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,

                    Position = new Vector2(10),
                    Text = "Rooms:"
                },
                new Rooms()
            });

            Add(controller = new TrackController
            {
                Alpha = 0,
                PassDownInput = false
            });
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }

        public class Rooms : Layer2D<IDrawable2D>
        {
            public Rooms()
            {

            }

            public class Room : Button
            {
                public Room()
                {
                    Text2D = new Text2D()
                }
            }
        }
    }
}