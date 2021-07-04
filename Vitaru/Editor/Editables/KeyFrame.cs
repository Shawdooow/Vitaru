﻿using System.Collections.Generic;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.Editables
{
    public class KeyFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(KeyFrame);

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> start.
        /// Remapped to between 0 - 1 for a given entity's StartTime to it's EndTime
        /// </summary>
        public double StartTime;

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> end.
        /// Remapped to between 0 - 1 for a given entity's StartTime to it's EndTime
        /// </summary>
        public double EndTime;

        public List<KeyFrameEffect> Effects = new();

        /// <summary>
        /// <see cref="current"/> should be remapped to between 0 - 1 for a given entity's StartTime to it's EndTime
        /// </summary>
        /// <param name="current"></param>
        public virtual void ApplyKeyFrame(double current)
        {
            double c = PrionMath.Remap(current, StartTime, EndTime);

            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Apply(c);
        }

        public abstract class KeyFrameEffect : IHasName
        {
            public virtual string Name { get; set; } = nameof(KeyFrameEffect);

            public virtual Easings Easing { get; set; } = Easings.None;

            public virtual void Apply(double current) => ApplyEffect(Prion.Nucleus.Utilities.Easing.ApplyEasing(Easing, current));

            protected abstract void ApplyEffect(double current);
        }
    }
}
