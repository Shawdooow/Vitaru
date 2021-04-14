// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Roots.Menu;
using Vitaru.Tracks;

namespace Vitaru.Roots
{
    public class LevelSelect : MenuRoot
    {
        public override string Name => nameof(LevelSelect);

        protected override bool UseLevelBackground => true;

        private readonly VitaruTrackController controller;

        public LevelSelect()
        {
            Add(new TrackSelect());
            Add(new CharacterSelect());
            Add(controller = new VitaruTrackController());
        }

        public override void Update()
        {
            base.Update();

            controller.Update();
            controller.TryRepeat();
        }
    }
}