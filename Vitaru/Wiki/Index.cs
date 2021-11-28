// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Gamemodes;
using Vitaru.Mods;
using Vitaru.Wiki.Included;

namespace Vitaru.Wiki
{
    public class Index : HoverableLayer<IDrawable2D>
    {
        public event Action<WikiPanel> OnSetPanel;

        public Index()
        {
            Alpha = 0.75f;

            ParentOrigin = Mounts.CenterLeft;
            Origin = Mounts.CenterLeft;

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

            List<WikiPanel> panels = new()
            {
                new VitaruWiki(),
                new MultiplayerWiki(),
                new EditorWiki(),
            };

            foreach (GamemodeStore.LoadedGamemode gamemode in GamemodeStore.LoadedGamemodes)
            {
                WikiPanel p = gamemode.Gamemode.GetWikiPanel();

                if (p != null)
                    panels.Add(p);
            }

            foreach (Mod mod in Modloader.LoadedMods)
            {
                WikiPanel p = mod.GetWikiPanel();

                if (p != null)
                    panels.Add(p);
            }

            foreach (WikiPanel p in panels)
            {
                list.Add(new Button(false)
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    //ParentSizing = Axes.Horizontal,
                    Width = 160,
                    Height = 24,

                    Text = p.Name,

                    Text2D =
                    {
                        ParentOrigin = Mounts.CenterLeft,
                        Origin = Mounts.CenterLeft,

                        FontScale = 0.28f,
                    },

                    OnClick = () => OnSetPanel?.Invoke(p),
                });
            }

            OnSetPanel?.Invoke(panels[0]);
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