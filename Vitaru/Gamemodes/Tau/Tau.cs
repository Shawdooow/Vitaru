﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Vitaru.Editor.IO;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Tau
{
    public class Tau : Gamemode
    {
        public override string Name => "Tau";

        public override FormatConverter GetFormatConverter()
        {
            throw new NotImplementedException();
        }

        public override Gamefield GetGamefield()
        {
            throw new NotImplementedException();
        }

        public override Chapter[] GetChapters()
        {
            throw new NotImplementedException();
        }
    }
}