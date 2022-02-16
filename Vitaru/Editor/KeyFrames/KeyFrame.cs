// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Linq;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public abstract class KeyFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(KeyFrame);

        /// <summary>
        ///     Time this KeyFrame fully takes effect.
        /// </summary>
        public double Time;

        /// <summary>
        /// Apply this frame's effect
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        public abstract void ApplyFrame(double current, KeyFrame next);

        /// <summary>
        /// List must be sorted by earliest to latest and only contain KeyFrames applying the same effect
        /// </summary>
        /// <param name="current"></param>
        /// <param name="frames"></param>
        public static void ApplyFrames(double current, List<KeyFrame> frames)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                KeyFrame f = frames[i];

                if (current <= f.Time && f == frames.First())
                    f.ApplyFrame(current, null);

                if (current >= f.Time)
                {
                    if (f == frames.Last())
                        f.ApplyFrame(current, null);

                    else
                        f.ApplyFrame(current, frames[i + 1]);
                }
            }
        }
    }

    public enum KeyFrameTypes
    {
        Position,
        Alpha
    }
}