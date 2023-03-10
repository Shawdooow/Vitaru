// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using System.Drawing;
using System.Numerics;
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
            Size = new Vector2(160, 420);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

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

            ScrollLayer list = new()
            {
                //ParentSizing = Axes.Both,
                Size = Size,
                Spacing = 2,
            };

            Add(list);

            foreach (LevelPack p in LevelStore.LoadedLevels)
            {
                list.List.Add(new Button
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,

                    //ParentSizing = Axes.Horizontal,
                    Width = 160 - 16,
                    Height = 18,

                    Text = p.Title,

                    Text2D =
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterLeft,

                        FontScale = 0.24f,
                        PreviousBuffer = MaskingLayer<IDrawable2D>.MaskBuffer
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

            list.List.Height = list.List.Children[0].Height * list.List.Children.Count;
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