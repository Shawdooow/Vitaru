// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Linq;
using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public abstract class KeyFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(KeyFrame);

        /// <summary>
        ///     Time this KeyFrame starts.
        /// </summary>
        public double StartTime;

        public Easings Easing = Easings.None;

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

                if (current <= f.StartTime && f == frames.First())
                    f.ApplyFrame(current, null);

                if (current >= f.StartTime)
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
        Alpha,
        Color,
        Size,
        Scale,
        Damage,
        Health,
        Active,
        Path,
    }
}