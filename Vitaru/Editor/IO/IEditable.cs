// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Core.Entitys;
using Vitaru.Gamemodes;

namespace Vitaru.Editor.IO
{
    public interface IEditable : IUpdatable
    {
        Vector2 Position { get; set; }

        void SetDrawable(DrawableGameEntity drawable);

        DrawableGameEntity GenerateDrawable();

        void ParseString(string[] data, int offset);

        string[] SerializeToStrings();
    }
}