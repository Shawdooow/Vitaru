// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using Vitaru.Editor.Editables.Properties.Time;

namespace Vitaru.Editor.KeyFrames
{
    public interface IHasKeyFrames : IHasStartTime, IHasEndTime
    {
        List<KeyFrame> KeyFrames { get; set; }

        protected void ApplyKeyFrames()
        {
            double current = Clock.Current;
            for (int i = 0; i < KeyFrames.Count; i++)
                KeyFrames[i].ApplyKeyFrame(current);
        }
    }
}