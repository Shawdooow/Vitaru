// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using System.Drawing;
using System.Numerics;
using Vitaru.Mods;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class ModsTest : MenuRoot
    {
        public override string Name => nameof(ModsTest);

        protected override bool UseLevelBackground => true;

        protected override bool Parallax => true;

        private VitaruTrackController controller;

        public override void RenderingLoadingComplete()
        {
            foreach (Mod mod in Modloader.LoadedMods)
            {
                Button b = mod.GetMenuButton();
                Root r = mod.GetRoot();
                if (b != null && r != null)
                {
                    r.Dispose();
                    Add(b);

                    if (mod.Disabled)
                    {
                        b.Add(new Box
                        {
                            Size = b.Size,
                            Scale = b.Scale,
                            Color = Color.Black,
                            Alpha = 0.5f,
                        });
                    }
                    else
                        b.OnClick += () => AddRoot(mod.GetRoot());
                }
            }

            Add(controller = new VitaruTrackController
            {
                Position = new Vector2(-40),
                Origin = Mounts.BottomRight,
                ParentOrigin = Mounts.BottomRight,
            });

            base.RenderingLoadingComplete();
        }

        protected override void OnResume()
        {
            base.OnResume();
            TrackManager.SetPositionalDefaults();
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }
    }
}