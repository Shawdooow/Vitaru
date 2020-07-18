// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Roots;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Mods;
using Vitaru.Tracks;

namespace Vitaru.Roots.Tests
{
    public class ModsTest : MenuRoot
    {
        public override string Name => nameof(ModsTest);

        protected override bool UseLevelBackground => true;

        private readonly TrackController controller;

        public ModsTest()
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
                            Alpha = 0.5f
                        });
                    }
                    else
                        b.OnClick += () => AddRoot(mod.GetRoot());
                }
            }

            Add(controller = new TrackController
            {
                Alpha = 0
            });
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Renderer.Window.CursorHidden = false;
        }
    }
}