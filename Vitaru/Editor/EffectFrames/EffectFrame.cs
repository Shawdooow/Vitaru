// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.EffectFrames
{
    public class EffectFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(EffectFrame);

        /// <summary>
        ///     Time this EffectFrame's <see cref="Effects" /> start.
        /// </summary>
        public double StartTime;

        /// <summary>
        ///     Time this EffectFrame's <see cref="Effects" /> end.
        /// </summary>
        public double EndTime;

        public List<Effect> Effects = new();

        /// <summary>
        /// </summary>
        /// <param name="current"></param>
        public virtual void ApplyKeyFrame(double current)
        {
            float c = (float)PrionMath.Remap(current, StartTime, EndTime);

            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Apply(c);
        }
    }
}