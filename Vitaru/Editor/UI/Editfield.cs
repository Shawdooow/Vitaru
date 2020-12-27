// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Mitochondria.Input.Receivers;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Editor.Editables.Properties.Position;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Enemies;
using Vitaru.Levels;
using Vitaru.Play;
using Vitaru.Tracks;

namespace Vitaru.Editor.UI
{
    public class Editfield : Gamefield, IHasInputMouseButtons
    {
        private readonly LevelManager manager;

        public readonly HoverableLayer<DrawableGameEntity> SelectionLayer = new HoverableLayer<DrawableGameEntity>
        {
            Size = new Vector2(1024, 820),
            Scale = new Vector2(0.5f),
            Clock = TrackManager.CurrentTrack.DrawClock
        };

        private bool clicked;

        public Editfield(LevelManager manager)
        {
            this.manager = manager;

            manager.GeneratorSet += g => manager.SetEditable(g.GetEditable(this));

            manager.EditableSet += Selected;

            manager.OnSerializeToLevel += () =>
            {
                List<Enemy> master = new List<Enemy>();
                master.AddRange(UnloadedEnemies);
                master.AddRange(LoadedEnemies);

                manager.Level.EnemyData = FormatConverter.EnemiesToString(master);
                LevelStore.SaveCurrentLevel();
            };

            ParticleLayer.Scale = new Vector2(0.5f);
            CharacterLayer.Scale = new Vector2(0.5f);
            BulletLayer.Scale = new Vector2(0.5f);
        }

        public void Selected(IEditable editable)
        {
            if (editable != null)
            {
                Enemy enemy = (Enemy) editable;
                if (enemy.Drawable == null)
                {
                    DrawableGameEntity draw = editable.GenerateDrawable();
                    editable.SetDrawable(draw);
                    enemy.Drawable.Scale = new Vector2(0.25f);
                    //Add(enemy);
                    LoadedEnemies.Add(enemy);
                    enemy.OnAddParticle = ParticleLayer.Add;
                }

                enemy.Drawable.ClearTransforms();
                enemy.Drawable.Alpha = 1;

                IDrawable2D outline = manager.SelectedEditable.GetOverlay(enemy.Drawable);
                enemy.Drawable.Add(outline);
                SelectionLayer.Child = enemy.Drawable;
            }
            else
            {
                while (SelectionLayer.Any())
                {
                    DrawableGameEntity c = SelectionLayer.Children[0];
                    c.Remove(c.Children.Last());
                    SelectionLayer.Remove(c, false);
                    CharacterLayer.Add(c);
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (clicked)
            {
                foreach (EditableProperty p in manager.Properties)
                    if (p is EditableStartPosition start)
                    {
                        Vector2 diff = InputManager.Mouse.Position - InputManager.LastMouse.Position;
                        start.SetValue(start.Value + diff * 2);
                    }
            }
        }

        public bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (Enemy ey in LoadedEnemies)
                {
                    if (ey.Drawable.Alpha > 0)
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

                            if (manager.SelectedEditable != ey)
                            {
                                manager.SetEditable(null);
                                CharacterLayer.Remove(ey.Drawable, false);
                                manager.SetEditable(ey);
                            }

                            return true;
                        }
                    }
                }

                if (SelectionLayer.Hovered)
                    manager.SetEditable(null);
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