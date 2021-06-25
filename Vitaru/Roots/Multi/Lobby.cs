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
using Vitaru.Levels;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;

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
                new Rooms(),
                new Button
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,

                    Size = new Vector2(200, 20),
                    Text = "Create Room",
                    OnClick = create
                },
                new Button
                {
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,

                    Size = new Vector2(200, 20),
                    Text = "Join Room",
                    OnClick = join
                },
                new Button
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,

                    Size = new Vector2(200, 20),
                    Text = "Refresh",
                    OnClick = refresh
                }
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

        private void create()
        {
            VitaruNet.SendPacketTcp(new CreateMatchPacket
            {
                MatchInfo = new MatchInfo
                {
                    Host = VitaruNet.VitaruUser.ID,

                    Level = LevelStore.CurrentLevel
                }
            });
        }

        private void join()
        {
            VitaruNet.SendPacketTcp(new JoinMatchPacket
            {
                //provide selected match on list
                Match = 0,
                User = VitaruNet.VitaruUser
            });
        }

        private void refresh()
        {
            VitaruNet.SendPacketTcp(new RequestMatchListPacket());
        }

        public class Rooms : InputLayer<IDrawable2D>
        {
            public Rooms()
            {

            }

            public class Room : Button
            {
                public Room()
                {
                    Text2D.Origin = Mounts.TopLeft;
                    Text2D.ParentOrigin = Mounts.TopLeft;
                }
            }
        }
    }
}