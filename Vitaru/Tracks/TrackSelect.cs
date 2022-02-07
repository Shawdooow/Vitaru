// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Levels;

namespace Vitaru.Tracks
{
    public class TrackSelect : HoverableLayer<IDrawable2D>
    {
        public TrackSelect()
        {
            Alpha = 0.75f;

            ParentOrigin = Mounts.TopLeft;
            Origin = Mounts.TopLeft;

            Position = new Vector2(8);
            Size = new Vector2(160, 800);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            ListLayer<Button> list;

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    ParentSizing = Axes.Both,
                },
            };

            list = new ListLayer<Button>
            {
                //ParentSizing = Axes.Both,
                Size = Size,
                Spacing = 2,
            };

            Add(new MaskingLayer<IDrawable2D>
            {
                Child = list,

                Masks = new Sprite[]
                {
                    new Box
                    {
                        Alpha = 0f,
                        Size = Size,
                    },
                },
            });

            foreach (LevelPack p in LevelStore.LoadedLevels)
            {
                list.Add(new Button(false)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    //ParentSizing = Axes.Horizontal,
                    Width = 160,
                    Height = 18,

                    Text = p.Title,

                    Text2D =
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterLeft,

                        FontScale = 0.24f,
                    },

                    OnClick = () =>
                    {
                        if (!TrackManager.Switching)
                        {
                            TrackManager.Switching = true;
                            Game.ScheduleLoad(() =>
                            {
                                TrackManager.PreviousTracks.Push(LevelStore.CurrentLevel.Metadata);
                                LevelStore.SetLevelPack(p);
                                TrackManager.SetTrack(p.Levels[0].Metadata);
                            });
                        }
                    },
                    OnRightClick = () => { LevelStore.NextLevelPack = p; },
                });
            }
        }

        public override void OnHovered()
        {
            base.OnHovered();
            this.FadeTo(1f, 200f);
        }

        public override void OnHoverLost()
        {
            base.OnHoverLost();
            this.FadeTo(0.75f, 200f);
        }
    }
}