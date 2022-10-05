// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Vitaru.Chapters.Abilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Five
{
    public class Muris : Player
    {
        public override string Name => "Muris";

        public override float HealthCapacity => 40;

        public override float EnergyCapacity => 36;

        public override float EnergyCost => 12;

        public override float EnergyDrainRate => 6;

        public override Color PrimaryColor => "#00cd00".HexToColor();

        public override Color SecondaryColor => "#c3ad00".HexToColor();

        public override Color ComplementaryColor => "#666666".HexToColor();

        public override string Seal => "Gameplay\\seal alki hell.png";

        public override float SealRotationSpeed => 0.5f;

        public override string Ability => "God of Rhythm";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.Extreme;

        public override ImplementationState ImplementationState => ImplementationState.PartiallyComplete;

        public override string OriginMedia => "Alki.Five";

        public override string Description => "";

        public Muris(PlayManager manager) : base(manager) { }

        protected Metronome Metronome;

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            //Gamefield.OverlaysLayer.Add(Metronome = new Metronome(this, Gamefield.OverlaysLayer));
        }
    }
}