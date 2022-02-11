// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Editor.Editables.Properties.Time;

namespace Vitaru.Editor.EffectFrames
{
    public interface IHasEffectFrames : IHasStartTime, IHasEndTime
    {
        List<EffectFrame> EffectFrames { get; set; }

        protected void ApplyEffectFrames()
        {
            double current = Clock.Current;
            for (int i = 0; i < EffectFrames.Count; i++)
                EffectFrames[i].ApplyKeyFrame(current);
        }
    }
}