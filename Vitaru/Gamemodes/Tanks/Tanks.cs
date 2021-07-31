﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Vitaru.Editor.Editables;
using Vitaru.Editor.IO;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Tanks
{
    public class Tanks : Gamemode
    {
        //Wii Play Tanks 2
        public override string Name => "Tanks";

        public override Texture Icon { get; }

        public override FormatConverter GetFormatConverter()
        {
            throw Debugger.NotImplemented("");
        }

        public override Gamefield GetGamefield()
        {
            throw Debugger.NotImplemented("");
        }

        public override Chapter[] GetChapters()
        {
            throw Debugger.NotImplemented("");
        }

        public override EditableGenerator[] GetGenerators()
        {
            throw Debugger.NotImplemented("");
        }
    }
}