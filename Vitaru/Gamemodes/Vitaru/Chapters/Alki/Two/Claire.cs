// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Vitaru.Chapters.Abilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two
{
    public class Claire : Player
    {
        #region Fields

        public override string Name => "Claire";

        public override float HealthCapacity => 40;

        public override float EnergyCapacity => 32;

        public override float EnergyCost => 16;

        public override Color PrimaryColor => "#d9c630".HexToColor();

        public override Color SecondaryColor => "#c9120f".HexToColor();

        public override Color ComplementaryColor => "#757670".HexToColor();

        public override string Ability => "Snap Shot";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Hard;

        protected Camera Camera;

        #endregion

        public Claire(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Gamefield.OverlaysLayer.Add(Camera = new Camera());
        }

        public override void Update()
        {
            base.Update();
            Camera.Position = InputManager.Mouse.Position;
        }
    }
}