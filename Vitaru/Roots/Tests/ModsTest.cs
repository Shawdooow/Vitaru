﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Game.Graphics.Roots;
using Prion.Game.Graphics.UserInterface;
using Vitaru.Mods;

namespace Vitaru.Roots.Tests
{
    public class ModsTest : MenuRoot
    {
        protected override bool UseLevelBackground => true;

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
                    b.OnClick += () => AddRoot(mod.GetRoot());
                }
            }
        }
    }
}