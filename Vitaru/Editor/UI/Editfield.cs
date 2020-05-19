// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using OpenTK.Input;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Input.Events;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Play;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield
    {
        private Vector2 offset = Vector2.Zero;

        public readonly SelectLayer SelectionLayer = new SelectLayer();

        private Editable editable;

        public void Selected(Editable edit)
        {
            editable = edit;
            IEditable e = editable.GetEditable(this);
            DrawableGameEntity draw = e.GenerateDrawable();
            IDrawable2D outline = edit.GetOverlay(draw);
            draw.Add(outline);

            e.SetDrawable(draw);
            Add(e as Enemy);
            SelectionLayer.Children = new[]
            {
                draw
            };
        }

        public override void Update()
        {
            base.Update();

            if (SelectionLayer.Right)
                foreach (IEditable e in LoadedEnemies)
                    e.Position += SelectionLayer.Cursor - offset;

            offset = SelectionLayer.Cursor;
        }

        public class SelectLayer : ClickableLayer<IDrawable2D>
        {
            public Vector2 Cursor { get; private set; }
            public bool Right { get; private set; }
            public bool Click;

            public override void OnMouseMove(MousePositionEvent e)
            {
                base.OnMouseMove(e);
                Cursor = e.Position;
            }

            public override bool OnMouseDown(MouseButtonEvent e)
            {
                switch (e.Button)
                {
                    case MouseButton.Right:
                        Right = true;
                        Cursor = e.Position;
                        break;
                    case MouseButton.Left:
                        Click = true;
                        break;
                }

                return base.OnMouseDown(e);
            }

            public override bool OnMouseUp(MouseButtonEvent e)
            {
                if (e.Button == MouseButton.Right)
                    Right = false;
                return base.OnMouseUp(e);
            }
        }
    }
}