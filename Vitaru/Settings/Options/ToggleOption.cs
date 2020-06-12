// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Input;
using Prion.Nucleus.IO.Configs;

namespace Vitaru.Settings.Options
{
    public class ToggleOption<T> : ToggleOption
        where T : struct, IConvertible
    {
        public override string Name { get; set; } = nameof(ToggleOption<T>);

        private readonly SettingsManager<T> manager;
        private readonly T setting;

        public ToggleOption(SettingsManager<T> manager, T setting)
        {
            this.manager = manager;
            this.setting = setting;

            Value = manager.GetBool(setting);
            Circle.Color = Value ? Color.LawnGreen : Color.Red;
        }

        protected override void Toggle()
        {
            base.Toggle();
            manager.SetValue(setting, Value);
            Circle.Color = Value ? Color.LawnGreen : Color.Red;
        }
    }

    public abstract class ToggleOption : ClickableLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(ToggleOption);

        public Action<bool> OnValueChange;

        public bool Value;

        public override Vector2 Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                Circle.Size = new Vector2(Size.Y * 0.9f);
            }
        }

        public readonly SpriteText Text;
        protected readonly Circle Circle;

        protected ToggleOption()
        {
            Children = new IDrawable2D[]
            {
                Text = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterLeft,
                    TextScale = 0.2f
                },
                Circle = new Circle
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterRight
                }
            };
        }

        protected virtual void Toggle()
        {
            Value = !Value;
            OnValueChange?.Invoke(Value);
        }

        protected override void OnMouseDown()
        {
            //base.OnMouseDown();
            if (InputManager.Mouse.Left && Hovered)
                Toggle();
        }
    }
}