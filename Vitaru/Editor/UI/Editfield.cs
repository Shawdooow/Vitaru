// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Input.Receivers;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Play;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield, IHasInputMouseButtons
    {
        private Vector2 offset = Vector2.Zero;

        private readonly LevelManager manager;

        public readonly InputLayer<IDrawable2D> SelectionLayer = new InputLayer<IDrawable2D>();

        private bool clicked;

        public Editfield(LevelManager manager)
        {
            this.manager = manager;

            manager.GeneratorSet += g =>
            {
                manager.SetEditable(g.GetEditable(this));
            };

            manager.EditableSet += Selected;
        }

        public void Selected(IEditable editable)
        {
            DrawableGameEntity draw = editable.GenerateDrawable();
            IDrawable2D outline = manager.SelectedGenerator.GetOverlay(draw);
            draw.Add(outline);

            editable.SetDrawable(draw);
            Add(editable as Enemy);
            SelectionLayer.Children = new[]
            {
                new TooltipLayer
                {
                    Text = editable.Name,
                    Size = draw.Size,
                    Child = draw
                }
            };
        }

        public override void Update()
        {
            base.Update();

            if (clicked)
            {
                foreach (EditableProperty p in manager.Properties)
                    if (p is EditableStartPosition start)
                        start.SetValue(start.Value + InputManager.Mouse.Position - offset);
            }
            offset = InputManager.Mouse.Position;
        }

        public bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (Enemy ey in LoadedEnemies)
                {
                    if (ey == manager.SelectedEditable)
                    {
                        Vector4 transform = new Vector4(InputManager.Mouse.Position, 1, 1);
                        Matrix4x4.Invert(ey.Drawable.TotalTransform, out Matrix4x4 inverse);

                        transform = Vector4.Transform(transform, inverse);
                        transform.X /= ey.Drawable.Size.X;
                        transform.Y /= ey.Drawable.Size.Y;

                        if (transform.X >= -0.5 && transform.X <= 0.5 && transform.Y >= -0.5 &&
                            transform.Y <= 0.5)
                        {
                            clicked = true;
                            return true;
                        }
                    }
                }
            }
            else
            {
                
            }

            return false;
        }

        public bool OnMouseUp(MouseButtonEvent e)
        {
            clicked = false;
            return true;
        }
    }
}