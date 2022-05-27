// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Vitaru.Chapters;
using Vitaru.Editor.IO;

namespace Vitaru.Gamemodes.Tau
{
    public class Tau : Gamemode
    {
        public override string Name => "Tau";

        public override Texture Icon { get; }

        public override FormatConverter GetFormatConverter() => throw Debugger.NotImplemented("");

        //public override Gamefield GetGamefield() => throw Debugger.NotImplemented("");

        public override Chapter[] GetChapters() => throw Debugger.NotImplemented("");
    }
}