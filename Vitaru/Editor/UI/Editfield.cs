// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Layers;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Play;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield
    {
        public readonly Pack<GameEntity> SelectionPack = new Pack<GameEntity>();
        public readonly ClickableLayer<DrawableGameEntity> SelectionLayer = new ClickableLayer<DrawableGameEntity>();

        public Editfield()
        {
            Add(SelectionPack);
        }

        public void Selected(Editable editable)
        {
            IEditable edit = editable.GetEditable();
            DrawableGameEntity draw = edit.GenerateDrawable();
            edit.SetDrawable(draw);
            SelectionLayer.Child = draw;
        }
    }
}