// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Prion.Mitochondria.Graphics.UI;
using Prion.Mitochondria.Input.Receivers;
using Prion.Nucleus.IO;
using Prion.Nucleus.Utilities;
using Vitaru.Settings.Overlays;

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

            Size = new Vector2(NucleusSettingsOverlay.WIDTH - 8, 40);
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

        public Action<float> OnValueChange;

        public virtual float Value
        {
            get => value;
            set
            {
                if (this.value == value) return;

                this.value = value;
                Slider.Progress = PrionMath.Remap(Value, Min, Max);
                TextBox.Text = Value.ToString();
                OnValueChange?.Invoke(Value);
            }
        }

        private float value;

        public string Text
        {
            get => InstancedText.Text;
            set => InstancedText.Text = value;
        }

        protected readonly Text2D InstancedText;
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
                InstancedText = new Text2D
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterLeft,
                    FontScale = 0.2f,
                    Y = -10
                },
                TextBox = new TextBox
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterRight,
                    Y = -10,
                    Size = new Vector2((NucleusSettingsOverlay.WIDTH - 8) / 2, 20),

                    InstancedText =
                    {
                        FontScale = 0.2f
                    },

                    OnEnter = text =>
                    {
                        try
                        {
                            float value = float.Parse(text);
                            PrionMath.Remap(value, Min, Max);
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
                    Size = new Vector2(NucleusSettingsOverlay.WIDTH - 24, 20),

                    OnProgressInput = p => Value = PrionMath.Remap(p, 0, 1, Min, Max)
                }
            };
        }
    }
}