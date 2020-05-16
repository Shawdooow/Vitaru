// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using OpenTK.Input;
using Prion.Application.Groups.Packs;
using Prion.Game.Graphics.Layers;
using Prion.Game.Input.Events;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Play;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield
    {
        public readonly Pack<GameEntity> SelectionPack = new Pack<GameEntity>();
        public readonly SelectLayer SelectionLayer = new SelectLayer();

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

        public class SelectLayer : ClickableLayer<DrawableGameEntity>
        {
            private Vector2 offset = Vector2.Zero;
            private bool right;

            public override void OnMouseMove(MousePositionEvent e)
            {
                base.OnMouseMove(e);

                if (right)
                {
                    foreach (DrawableGameEntity entity in Children)
                    {
                        entity.Position += e.Position - offset;
                        offset = e.Position;
                    }
                }
            }

            public override bool OnMouseDown(MouseButtonEvent e)
            {
                if (e.Button == MouseButton.Right)
                {
                    right = true;
                    offset = e.Position;
                }
                return base.OnMouseDown(e);
            }

            public override bool OnMouseUp(MouseButtonEvent e)
            {
                if (e.Button == MouseButton.Right)
                    right = false;
                return base.OnMouseUp(e);
            }
        }
    }
}