// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Editor.IO;

namespace Vitaru.Gamemodes.Vitaru
{
    public class Vitaru : Gamemode
    {
        public override FormatConverter GetFormatConverter() => new VitaruConverter();
    }
}