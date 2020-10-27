using System;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input.Receivers;
using Prion.Nucleus.IO.Configs;
using Prion.Nucleus.Utilities;

namespace Vitaru.Settings.Options
{
    public class SliderOption<T> : SliderOption, IHasPollInput
        where T : struct, IConvertible
    {
        public override string Name { get; set; } = nameof(SliderOption<T>);

        public override float Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                if (!Slider.Dragging) manager.SetValue(setting, Value);
            }
        }

        private readonly SettingsManager<T> manager;
        private readonly T setting;

        public SliderOption(SettingsManager<T> manager, T setting, float min, float max) : base(min, max)
        {
            this.manager = manager;
            this.setting = setting;

            Value = manager.GetFloat(setting);

            Size = new Vector2(SettingsOverlay.WIDTH - 8, 40);
        }

        private bool dragging;

        public void PollInput()
        {
            if (dragging && !Slider.Dragging)
                manager.SetValue(setting, Value);
            dragging = Slider.Dragging;
        }
    }

    public abstract class SliderOption : InputLayer<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(SliderOption);

        public readonly float Min;
        public readonly float Max;

        public virtual float Value
        {
            get => value;
            set
            {
                if (this.value == value) return;

                this.value = value;
                Slider.Progress = PrionMath.Scale(Value, Min, Max);
                TextBox.Text = Value.ToString();
            }
        }

        private float value;

        public string Text
        {
            get => SpriteText.Text;
            set => SpriteText.Text = value;
        }

        protected readonly SpriteText SpriteText;
        protected readonly TextBox TextBox;
        protected readonly Slider Slider;

        protected SliderOption(float min, float max)
        {
            Min = min;
            Max = max;

            ParentOrigin = Mounts.TopCenter;
            Origin = Mounts.TopCenter;

            Children = new IDrawable2D[]
            {
                SpriteText = new SpriteText
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterLeft,
                    TextScale = 0.2f,
                    Y = -10,
                },
                TextBox = new TextBox
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterRight,
                    Y = -10,
                    Size = new Vector2((SettingsOverlay.WIDTH - 8) / 2, 20),

                    SpriteText =
                    {
                        TextScale = 0.2f
                    },

                    OnEnter = text =>
                    {
                        try
                        {
                            float value = float.Parse(text);
                            PrionMath.Scale(value, Min, Max);
                            Value = value;
                        }
                        catch
                        {
                            TextBox.Text = Value.ToString();
                        }
                    }
                },
                Slider = new Slider
                {
                    Y = 10,
                    Size = new Vector2(SettingsOverlay.WIDTH - 8, 20),

                    OnProgressInput = p => Value = PrionMath.Scale(p, 0, 1, Min, Max)
                }
            };
        }
    }
}
