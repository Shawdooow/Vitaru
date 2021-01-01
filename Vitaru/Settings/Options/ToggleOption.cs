// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Events;
using Prion.Nucleus.IO.Configs;
using Vitaru.Themes;

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
            Circle.Color = Value ? ThemeManager.PrimaryColor : ThemeManager.SecondaryColor;

            Size = new Vector2(SettingsOverlay.WIDTH - 8, 20);
        }

        protected override void Toggle()
        {
            base.Toggle();
            manager.SetValue(setting, Value);
            Circle.Color = Value ? ThemeManager.PrimaryColor : ThemeManager.SecondaryColor;
        }
    }

    public abstract class ToggleOption : Button
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

        protected readonly Circle Circle;

        protected ToggleOption()
        {
            ParentOrigin = Mounts.TopCenter;
            Origin = Mounts.TopCenter;

            InstancedText.ParentOrigin = Mounts.CenterLeft;
            InstancedText.Origin = Mounts.CenterLeft;
            InstancedText.FontScale = 0.2f;

            Add(Circle = new Circle
            {
                ParentOrigin = Mounts.CenterRight,
                Origin = Mounts.CenterRight
            });
        }

        protected virtual void Toggle()
        {
            Value = !Value;
            OnValueChange?.Invoke(Value);
        }

        public override bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButtons.Left && Hovered)
                Toggle();
            return base.OnMouseDown(e);
        }
    }
}