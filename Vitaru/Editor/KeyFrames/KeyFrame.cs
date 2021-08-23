// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class KeyFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(KeyFrame);

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> start.
        /// </summary>
        public double StartTime;

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> end.
        /// </summary>
        public double EndTime;

        public List<KeyFrameEffect> Effects = new();

        /// <summary>
        /// 
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
