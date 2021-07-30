// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Centrosome.NetworkingHandlers;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Levels;
using Vitaru.Networking.Client;
using Vitaru.Server.Match;
using Vitaru.Server.Packets.Lobby;

namespace Vitaru.Roots.Multi
{
    public class Lobby : MultiRoot
    {
        protected override bool Parallax => true;

        private readonly Rooms rooms;

        private readonly TrackController controller;

        private uint selected;

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
                rooms = new Rooms(value => selected = value),
                new Button
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomLeft,

                    Size = new Vector2(240, 40),
                    Text = "Create Room",
                    OnClick = create
                },
                new Button
                {
                    ParentOrigin = Mounts.BottomCenter,
                    Origin = Mounts.BottomCenter,

                    Size = new Vector2(240, 40),
                    Text = "Join Room",
                    OnClick = join
                },
                new Button
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomRight,

                    Size = new Vector2(240, 40),
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

        protected override void OnPacketRecieve(PacketInfo<VitaruHost> info)
        {
            base.OnPacketRecieve(info);

            switch (info.Packet)
            {
                case MatchListPacket list:
                    rooms.RefreshRooms(list);
                    break;
                case MatchCreatedPacket created:
                    selected = created.MatchInfo.ID;
                    join();
                    break;
            }
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
                Match = selected,
                User = VitaruNet.VitaruUser
            });
        }

        private void refresh()
        {
            VitaruNet.SendPacketTcp(new RequestMatchListPacket());
        }

        public class Rooms : InputLayer<IDrawable2D>
        {
            private const float width = 1200;
            private const float height = 600;

            private Action<uint> select;
            private ListLayer<Room> rooms;

            public Rooms(Action<uint> select)
            {
                this.select = select;

                ParentOrigin = Mounts.TopCenter;
                Origin = Mounts.TopCenter;

                Size = new Vector2(width, height);
                Y = 100;
            }

            public void RefreshRooms(MatchListPacket list)
            {
                Children = new IDrawable2D[]
                {
                    rooms = new ListLayer<Room>
                    {
                        ParentOrigin = Mounts.Center,
                        Origin = Mounts.Center,

                        Size = new Vector2(width, height)
                    }
                };

                for (int i = 0; i < list.MatchInfos.Count; i++)
                    rooms.Add(new Room(list.MatchInfos[i], select)
                    {
                        Size = new Vector2(width, height / 8)
                    });
            }

            public class Room : Button
            {
                public Room(MatchInfo match, Action<uint> select)
                {
                    ParentOrigin = Mounts.TopCenter;
                    Origin = Mounts.TopCenter;

                    Text2D.Origin = Mounts.TopLeft;
                    Text2D.ParentOrigin = Mounts.TopLeft;

                    Text2D.Text = match.Name;

                    OnClick = () => select.Invoke(match.ID);
                }
            }
        }
    }
}