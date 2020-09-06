// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Input;
using Vitaru.Editor.Editables;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Play;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield
    {
        private Vector2 offset = Vector2.Zero;

        public readonly InputLayer<IDrawable2D> SelectionLayer = new InputLayer<IDrawable2D>();

        private Editable editable;

        public Editfield(LevelManager manager)
        {
            manager.EditableSelected += Selected;
        }

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
                new TooltipLayer
                {
                    Text = e.Name,
                    Size = draw.Size,
                    Child = draw
                }
            };
        }

        public override void Update()
        {
            base.Update();

            if (InputManager.Mouse[MouseButtons.Left])
                foreach (Enemy e in LoadedEnemies)
                {
                    if (e.Drawable.Alpha > 0 && e is IEditable ed)
                    {
                        Vector4 transform = new Vector4(InputManager.Mouse.Position, 1, 1);
                        Matrix4x4.Invert(e.Drawable.TotalTransform, out Matrix4x4 inverse);

                        transform = Vector4.Transform(transform, inverse);
                        transform.X /= e.Drawable.Size.X;
                        transform.Y /= e.Drawable.Size.Y;

                        if (transform.X >= -0.5 && transform.X <= 0.5 && transform.Y >= -0.5 && transform.Y <= 0.5)
                            e.Position += InputManager.Mouse.Position - offset;
                    }
                }

            offset = InputManager.Mouse.Position;
        }
    }
}