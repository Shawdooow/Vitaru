// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Nucleus.Utilities;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Three
{
    public class Jack : Player
    {
        public override string Name => "The Horseman";

        public override Color PrimaryColor => "#ff7c00".HexToColor();

        public override Color SecondaryColor => "#000000".HexToColor();

        public override Color ComplementaryColor => "#dec327".HexToColor();

        public override string Ability => "Pumpkin Chunkin'";

        public override Role Role => Role.Offense;

        public override string OriginMedia => "Alki.Three";

        public Jack(Gamefield gamefield) : base(gamefield) { }
    }
}