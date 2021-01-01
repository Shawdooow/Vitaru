// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Nucleus.Entitys;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Gamemodes;

namespace Vitaru.Editor.Editables
{
    public interface IEditable : IUpdatable
    {
        bool Selected { get; set; }

        Vector2 Position { get; set; }

        EditableProperty[] GetProperties();

        void SetDrawable(DrawableGameEntity drawable);

        DrawableGameEntity GenerateDrawable();

        IDrawable2D GetOverlay(DrawableGameEntity draw);

        void ParseString(string[] data, int offset);

        string[] SerializeToStrings();
    }
}