using System;
using System.Drawing;
using System.Numerics;
using OpenToolkit.Windowing.Common.Input;
using Prion.Core.IO.Configs;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Prion.Game.Graphics.Sprites;
using Prion.Game.Graphics.Text;
using Prion.Game.Input.Events;

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
                    Origin = Mounts.CenterRight,
                }
            };
        }

        protected virtual void Toggle()
        {
            Value = !Value;
            OnValueChange?.Invoke(Value);
        }

        public override bool OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button == MouseButton.Left && Hovered)
                Toggle();
            return base.OnMouseDown(e);
        }
    }
}
